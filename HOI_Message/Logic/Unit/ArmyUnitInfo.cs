using System.IO;
using System.Collections.Generic;
using System.Linq;
using NLog;
using HOI_Message.Logic.Util.CWTool;
using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.Country;

namespace HOI_Message.Logic.Unit;

/// <summary>
/// 一个国家的所有部队数据
/// </summary>
/// <see cref="NationalInfo"/>
public class ArmyUnitInfo : UnitInfoBase
{
    public override int UnitSum => _map.Values.Sum(x => x.Count);
    public override CountryTag OwnCountryTag { get; } = CountryTag.Empty;
    public static ArmyUnitInfo Empty { get; } = new();

    // Key是模板名称
    private readonly Dictionary<string, Unit> _map = new(8);
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 从文件的绝对路径中构建一个陆军单位信息类
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <param name="countryTag">国家Tag</param>
    /// <exception cref="ParseException">当文件解析失败时</exception>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public ArmyUnitInfo(string filePath, CountryTag countryTag)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath} 不存在");
        }
        var root = new CWToolsAdapter(filePath);
        if (!root.IsSuccess)
        {
            throw new ParseException(root.ErrorMessage);
        }

        if (!root.Root.Has(Key.DivisionTemplate) || !root.Root.Has(Key.Units))
        {
            _map.TrimExcess();
            return;
        }

        OwnCountryTag = countryTag;
        // 添加所有部队模板
        var map = new Dictionary<string, ushort>();
        var unitTemplates = root.Root.Childs(Key.DivisionTemplate);
        foreach (var unitTemplate in unitTemplates)
        {
            string typeName = unitTemplate.Leafs(Key.Name).Last().Value.ToRawString();
            if (!map.TryAdd(typeName, 0))
            {
                _logger.Warn($"{filePath} 存在重复的name {typeName}");
            }
        }

        // 统计开局部署军队数量
        var unitsNode = root.Root.Child(Key.Units).Value;
        var divisions = unitsNode.Childs("division");

        foreach (var division in divisions)
        {
            var unitName = division.Leafs(Key.DivisionTemplate).Last().Value.ToRawString();
            if (map.ContainsKey(unitName))
            {
                map[unitName]++;
            }
            else
            {
                _logger.Warn($"不存在的单位模板 '{unitName}'");
            }
        }

        foreach (var item in map)
        {
            _map[item.Key] = new Unit(item.Key, item.Value);
        }
        _map.TrimExcess();
    }

    private ArmyUnitInfo()
    {
        _map.TrimExcess();
    }

    private sealed class Unit
    {
        public string TypeName { get; }
        public ushort Count { get; }

        public Unit(string typeName, ushort count = 0)
        {
            TypeName = typeName;
            Count = count;
        }
    }

    private static class Key
    {
        public const string Name = "name";
        public const string Units = "units";
        public const string DivisionTemplate = "division_template";
    }
}
