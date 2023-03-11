using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.State;
using HOI_Message.Logic.Unit;
using HOI_Message.Logic.Util.CWTool;
using SkiaSharp;

namespace HOI_Message.Logic.Country;

/// <summary>
/// 保存着一个国家的所有信息
/// </summary>
public partial class NationalInfo
{
    /// <summary>
    /// 保存着游戏中所有已定义的国家 Tag
    /// </summary>
    private static HashSet<CountryTag> _tagSet { get; } = new();
    public CountryTag Tag { get; private set; }

    /// <summary>
    /// 一个国家的总人口
    /// </summary>
    public long ManpowerSum => _manpower.Value;

    /// <summary>
    /// 一个国家控制的所有地块数量
    /// </summary>
    public int OwnStatesNumber => _states.Count;
    public string OOBName { get; private set; }

    /// <summary>
    /// 科研槽数量
    /// </summary>
    public byte ResearchSlotsNumber { get; private set; }
    public int ConvoysNumber { get; private set; }

    /// <summary>
    /// 当前执政党
    /// </summary>
    public string RulingParty { get; }
    public ArmyUnitInfo ArmyUnitInfo => _armyUnitInfo;
    public SKColor? MapColor { get; private set; }
    public static NationalInfo Empty { get; } = new();
    public IEnumerable<StateInfo> StateInfos => _states;

    private readonly List<StateInfo> _states;
    private readonly Lazy<long> _manpower;
    private readonly Lazy<Dictionary<string, uint>> _buildings;
    private readonly Lazy<Dictionary<string, uint>> _resources;
    private ArmyUnitInfo _armyUnitInfo = ArmyUnitInfo.Empty;

    public NationalInfo(CountryFileParser parser, List<StateInfo> states, string? tag = null)
    {
        _states = states;
        SetTagProperty(tag);

        _manpower = new Lazy<long>(_states.Sum(x => x.Manpower));
        _buildings = new Lazy<Dictionary<string, uint>>(() => LazyInitHelper.GetAllBuildingsSumLazy(_states));
        _resources = new Lazy<Dictionary<string, uint>>(() => LazyInitHelper.GetAllResourcesSumLazy(_states));
        OOBName = parser.OOBName;
        ResearchSlotsNumber = parser.ResearchSlotsNumber;
        ConvoysNumber = parser.ConvoysNumber;
        RulingParty = parser.RulingParty;

        _states.TrimExcess();
    }

    private void SetTagProperty(string? tag)
    {
        if (tag is null)
        {
            Tag = _states.Count != 0 ? _states.First().OwnerTag : new CountryTag("ERR");
        }
        else
        {
            Tag = new CountryTag(tag);
        }
    }

    /// <summary>
    /// 按地区控制国家进行分类
    /// </summary>
    /// <param name="states">游戏中所有地块数据</param>
    /// <returns>Key是国家 Tag, Value是国家控制的地块集合</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Dictionary<CountryTag, List<StateInfo>> ClassifyStates(IEnumerable<StateInfo> states)
    {
        var map = new Dictionary<CountryTag, List<StateInfo>>(128);

        foreach (var state in states)
        {
            if (map.TryGetValue(state.OwnerTag, out var message))
            {
                message.Add(state);
            }
            else
            {
                var list = new List<StateInfo>(16) { state };
                map.Add(state.OwnerTag, list);
            }
        }

        return map;
    }

    private NationalInfo()
    {
        Tag = CountryTag.Empty;
        OOBName = string.Empty;
        RulingParty = string.Empty;
        _states = new List<StateInfo>();
        _manpower = new Lazy<long>(0L);
        _buildings = new Lazy<Dictionary<string, uint>>();
        _resources = new Lazy<Dictionary<string, uint>>();
    }

    public uint GetBuildingsSum(string buildingsType)
    {
        return _buildings.Value.TryGetValue(buildingsType, out uint value) ? value : 0;
    }

    public uint GetResourcesSum(string resourcesType)
    {
        return _resources.Value.TryGetValue(resourcesType, out uint value) ? value : 0;
    }

    public IList<string> GetAllResourcesTypes()
    {
        return new List<string>(_resources.Value.Keys);
    }


    /// <summary>
    /// 获得国家的 Tag 和对应的文件绝对路径
    /// </summary>
    /// <param name="gameRootPath">游戏根目录</param>
    /// <returns>Key是国家Tag, Value是国家文件绝对路径</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Dictionary<CountryTag, string> GetAllCountriesTag(string gameRootPath)
    {
        string folderPath = Path.Combine(gameRootPath, "history", "countries");
        var map = new Dictionary<CountryTag, string>(128);
        var files = new DirectoryInfo(folderPath).GetFiles(".", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var countryTag = file.Name[..3];
            map[new CountryTag(countryTag)] = file.FullName;
        }

        return map;
    }

    /// <summary>
    /// 获得已注册国家Tag和国家版图颜色文件的路径
    /// </summary>
    /// <param name="gameRootPath"></param>
    /// <returns>Key是国家Tag, Value是国家颜色文件的绝对路径</returns>
    /// <exception cref="ParseException"></exception>
    public static Dictionary<CountryTag, string> GetCountriesColorFilePath(string gameRootPath)
    {
        const string Common = "common";
        var folderPath = Path.Combine(gameRootPath, Common, "country_tags");
        var files = new DirectoryInfo(folderPath).GetFiles();
        var map = new Dictionary<CountryTag, string>();

        foreach (var file in files)
        {
            var root = new CWToolsAdapter(file.FullName);
            if (!root.IsSuccess)
            {
                throw new ParseException("解析错误");
            }

            if (root.Root.Has("dynamic_tags"))
            {
                if (root.Root.Leafs("dynamic_tags").First().Value.ToRawString() == "yes")
                {
                    continue;
                }
            }

            foreach (var leaf in root.Root.Leaves)
            {
                var paths = leaf.Value.ToRawString().Split('/');
                map[new CountryTag(leaf.Key)] = Path.Combine(gameRootPath, Common, paths[0], paths[1]);
            }
        }

        _tagSet.UnionWith(map.Keys);
        return map;
    }

    /// <summary>
    /// 按文件绝对路径设置单位数据
    /// </summary>
    /// <remarks>
    /// 单位信息文件在 history/units 文件夹下
    /// </remarks>
    /// <param name="filePath">文件绝对路径</param>
    /// <exception cref="ParseException"></exception>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public void SetUnitInfo(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath} 不存在");
        }
        _armyUnitInfo = new ArmyUnitInfo(filePath, Tag);
    }

    public void SetMapColor(byte red, byte green, byte blue)
    {
        MapColor = new SKColor(red, green, blue);
    }

    public static bool HasTag(CountryTag tag)
    {
        return _tagSet.Contains(tag);
    }
}
