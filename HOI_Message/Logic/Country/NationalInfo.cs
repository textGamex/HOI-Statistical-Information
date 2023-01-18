using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HOI_Message.Logic.State;
using HOI_Message.Logic.Unit;
using HOI_Message.Logic.Util.CWTool;

namespace HOI_Message.Logic.Country;

/// <summary>
/// 保存着一个国家的所有信息
/// </summary>
public class NationalInfo
{
    public string Tag { get; private set; }
    public long Manpower => _manpower.Value;
    public int OwnStatesNumber => _states.Count;
    public string OOBName { get; private set; }
    public byte ResearchSlotsNumber { get; private set; }
    public int ConvoysNumber { get; private set; }
    public UnitInfo UnitInfo { get; set; }
    public string RulingParty { get; }


    private readonly List<StateInfo> _states;
    private readonly Lazy<long> _manpower;
    private readonly Lazy<Dictionary<string, uint>> _buildings;
    private readonly Lazy<Dictionary<string, uint>> _resources;

    public NationalInfo(CountryFileParser parser, List<StateInfo> states, string? tag = null)
    {
        _states = states ?? throw new ArgumentNullException(nameof(states));
        if (tag is null)
        {
            if (_states.Count != 0)
            {
                Tag = _states.First().OwnerTag;
            }
            else
            {
                Tag = "NONE";
            }
        }
        else
        {
            Tag = tag;
        }

        _manpower = new Lazy<long>(_states.Sum(x => x.Manpower));
        _buildings = new Lazy<Dictionary<string, uint>>(GetAllBuildingsSumLazy);
        _resources = new Lazy<Dictionary<string, uint>>(GetAllResourcesSumLazy);
        OOBName = parser.OOBName;
        ResearchSlotsNumber = parser.ResearchSlotsNumber;
        ConvoysNumber = parser.ConvoysNumber;
        RulingParty = parser.RulingParty;

        _states.TrimExcess();
    }

    /// <summary>
    /// 按地区控制国家进行分类
    /// </summary>
    /// <param name="states">游戏中所有地块数据</param>
    /// <returns>Key是国家 Tag, Value是国家控制的地块集合</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Dictionary<string, List<StateInfo>> ClassifyStates(IEnumerable<StateInfo> states)
    {
        var map = new Dictionary<string, List<StateInfo>>(128);

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

    public uint GetBuildingsSum(string buildingsType)
    {
        return _buildings.Value.TryGetValue(buildingsType, out uint vaule) ? vaule : 0;
    }

    public uint GetResourcesSum(string resourcesType)
    {
        return _resources.Value.TryGetValue(resourcesType, out uint vaule) ? vaule : 0;
    }

    public IList<string> GetAllResourcesTypes()
    {
        return new List<string>(_resources.Value.Keys);
    }

    private Dictionary<string, uint> GetAllBuildingsSumLazy()
    {
        var map = new Dictionary<string, uint>(8);
        foreach (var state in _states)
        {
            foreach (var type in state.Buildings.Keys)
            {
                if (map.TryGetValue(type, out var value))
                {
                    map[type] = value + state.Buildings[type];
                }
                else
                {
                    map.Add(type, state.Buildings[type]);
                }
            }
        }

        map.TrimExcess();
        return map;
    }

    private Dictionary<string, uint> GetAllResourcesSumLazy()
    {
        var map = new Dictionary<string, uint>(8);
        foreach (var state in _states)
        {
            foreach (var type in state.Resources.Keys)
            {
                if (map.TryGetValue(type, out var value))
                {
                    map[type] = value + state.Resources[type];
                }
                else
                {
                    map.Add(type, state.Resources[type]);
                }
            }
        }

        map.TrimExcess();
        return map;
    }

    /// <summary>
    /// 获得游戏内所有国家的 Tag 和对应的文件绝对路径
    /// </summary>
    /// <param name="gameRootPath">游戏根目录</param>
    /// <returns>Key是国家Tag, Value是国家文件绝对路径</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Dictionary<string, string> GetAllCountriesTag(string gameRootPath)
    {
        string folderPath = Path.Combine(gameRootPath, "history", "countries");
        var map = new Dictionary<string, string>(128);
        var files = new DirectoryInfo(folderPath).GetFiles(".", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var countryTag = file.Name[0..3];
            map[countryTag] = file.FullName;
        }

        return map;
    }
}
