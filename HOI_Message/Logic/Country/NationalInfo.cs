using HOI_Message.Logic.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HOI_Message.Logic.Country;

public class NationalInfo
{
    public string Tag { get; }        
    public long Manpower => _manpower.Value;
    public int OwnStatesNumber => _states.Count;
    private readonly List<StateInfo> _states;
    private readonly Lazy<long> _manpower;
    private readonly Lazy<Dictionary<string, uint>> _buildings;
    private readonly Lazy<Dictionary<string, uint>> _resources;

    public NationalInfo(List<StateInfo> states)
    {
        _states = states ?? throw new ArgumentNullException(nameof(states));
        Tag = states.First()?.OwnerTag ?? "NONE";
        _states.TrimExcess();

        _manpower = new Lazy<long>(_states.Sum(x => x.Manpower));
        _buildings = new Lazy<Dictionary<string, uint>>(GetAllBuildingsSumLazy);
        _resources = new Lazy<Dictionary<string, uint>>(GetAllResourcesSumLazy);
    }

    /// <summary>
    /// 根据<see cref="StateInfo"/>所属国家分类
    /// </summary>
    /// <param name="states"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static List<NationalInfo> ByStates(IList<StateInfo> states)
    {
        if (states is null)
        {
            throw new ArgumentNullException(nameof(states));
        }

        var map = new Dictionary<string, List<StateInfo>>(states.Count);
        var nationalMessage = new List<NationalInfo>();

        foreach (var state in states)
        {
            if (map.TryGetValue(state.OwnerTag, out List<StateInfo> message))
            {
                message.Add(state);
            }
            else
            {
                var list = new List<StateInfo>() { state };
                map.Add(state.OwnerTag, list);
            }
        }
        foreach (var item in map.Values)
        {
            nationalMessage.Add(new NationalInfo(item));
        }
        return nationalMessage;
    }

    public static List<NationalInfo> ByFolderPath(string folderPath)
    {
        var files = new DirectoryInfo(folderPath).GetFiles();
        var states = new List<StateInfo>(files.Length);
        foreach (var file in files)
        {
            states.Add(new StateInfo(file.FullName));
        }
        return ByStates(states);
    }

    public uint GetBuildingsSum(string buildingsType)
    {
        if (_buildings.Value.TryGetValue(buildingsType, out uint vaule))
        {
            return vaule;
        }
        else
        {
            return 0;
        }
    }

    public uint GetResourcesSum(string resourcesType)
    {
        if (_resources.Value.TryGetValue(resourcesType, out uint vaule))
        {
            return vaule;
        }
        else
        {
            return 0;
        }
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
        return map;
    }
}
