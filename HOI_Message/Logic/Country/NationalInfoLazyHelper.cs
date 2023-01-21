using HOI_Message.Logic.State;
using System.Collections.Generic;

namespace HOI_Message.Logic.Country;

public partial class NationalInfo
{
    /// <summary>
    /// 帮助惰性初始化的辅助类
    /// </summary>
    private static class LazyInitHelper
    {
        public static Dictionary<string, uint> GetAllBuildingsSumLazy(IEnumerable<StateInfo> states)
        {
            var map = new Dictionary<string, uint>(8);
            foreach (var state in states)
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

        public static Dictionary<string, uint> GetAllResourcesSumLazy(IEnumerable<StateInfo> states)
        {
            var map = new Dictionary<string, uint>(8);
            foreach (var state in states)
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
    }
}