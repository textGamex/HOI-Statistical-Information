using CWTools.Process;
using NLog;
using HOI_Message.Logic.Util.CWTool;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using HOI_Message.Logic.CustomException;

namespace HOI_Message.Logic.State;

public partial class StateInfo
{
    private class StateFileParser
    {
        private readonly CWToolsAdapter _root;
        private readonly Node _state;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static class Key
        {
            public const string State = "state";
            public const string Id = "id";
            public const string History = "history";
            public const string OwnerTag = "owner";
            public const string Manpower = "manpower";
            public const string AddCore = "add_core_of";
            public const string Resources = "resources";
            public const string Buildings = "buildings";
            public const string Name = "name";
        }

        /// <summary>
        /// 按文件绝对路径解析
        /// </summary>
        /// <remarks>
        /// 地块文件在 Hearts of Iron IV\history\states 下
        /// </remarks>
        /// <param name="filePath">地块的绝对路径</param>
        /// <exception cref="ParseException">当文件解析失败时</exception>
        /// <exception cref="ArgumentException">当文件为空时</exception>
        /// <exception cref="FileNotFoundException">当文件不存在时</exception>
        public StateFileParser(string filePath)
        {
            var adapter = new CWToolsAdapter(filePath);
            if (adapter.IsSuccess)
            {
                _root = adapter;
            }
            else
            {
                throw new ParseException($"文件解析错误, 路径: {filePath},错误信息:{adapter.ErrorMessage}");
            }

            if (_root.Root.Has(Key.State))
            {
                _state = _root.Root.Child(Key.State).Value;
            }
            else
            {
                throw new ArgumentException($"文件为空, 路径: {filePath}");
            }
        }

        public int GetId()
        {
            if (_state.Has(Key.Id))
            {
                var ids = _state.Leafs(Key.Id);
                if (ids.Count() != 1)
                {
                    _logger.Warn("{0} 不唯一", Key.Id);
                }

                string value = ids.Last().Value.ToRawString();
                try
                {
                    return int.Parse(value);
                }
                catch (FormatException ex)
                {
                    _logger.Error(ex, "进行转换的值: {0}", value);
                    throw ex;
                }
            }
            else
            {
                throw new ArgumentException($"缺少: {Key.Id}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string GetOwnerTag()
        {
            if (TryGetHistoryNode(out var history) && history.Has(Key.OwnerTag))
            {
                var ownerTags = history.Leafs(Key.OwnerTag);
                if (ownerTags.Count() != 1)
                {
                    _logger.Warn("{0} 不唯一", Key.OwnerTag);
                }
                var ownerTag = ownerTags.Last().Value.ToRawString();
                return ownerTag;
            }

            throw new ArgumentException($"缺少: {Key.OwnerTag}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public uint GetManpower()
        {
            if (!_state.Has(Key.Manpower))
            {
                throw new ArgumentException($"缺少: {Key.Manpower}");
            }

            var manpowerList = _state.Leafs(Key.Manpower);
            if (manpowerList.Count() != 1)
            {
                _logger.Warn("{}不是唯一的", Key.Manpower);
            }

            string value = manpowerList.Last().Value.ToRawString();
            try
            {
                return uint.Parse(value);
            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "进行转换的值: {0}", value);
                throw ex;
            }
        }

        public IList<string> GetHasCoreCountryTags()
        {
            var tags = new List<string>(8);
            if (TryGetHistoryNode(out var history) && history.Has(Key.AddCore))
            {
                foreach (var leaf in history.Leafs(Key.AddCore))
                {
                    tags.Add(leaf.Value.ToRawString().Trim());
                }
                tags.TrimExcess();
            }
            return tags;
        }

        /// <summary>
        /// 尝试获得<c>history</c>节点, 如果有多个, 返回最后一个.
        /// </summary>
        /// <param name="node"><c>history</c>节点</param>
        /// <returns>如果成功, 返回<c>true</c>, 否则, 返回<c>false</c>.</returns>
        private bool TryGetHistoryNode(out Node node)
        {
            if (_state.Has(Key.History))
            {
                var histories = _state.Childs(Key.History);
                if (histories.Count() != 1)
                {
                    _logger.Warn("{0}不唯一", Key.History);
                }
                node = histories.Last();
                return true;
            }

            node = Node.Create(string.Empty);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>        
        public IDictionary<string, ushort> GetResourcesMap()
        {
            var map = new Dictionary<string, ushort>(8);
            if (_state.Has(Key.Resources))
            {
                var resources = _state.Child(Key.Resources).Value;
                foreach (var item in resources.Leaves)
                {
                    // 为了去除小数点
                    var temp = double.Parse(item.Value.ToRawString()).ToString("F0");
                    ushort value = ushort.Parse(temp);
                    map.Add(item.Key, value);
                }
            }
            map.TrimExcess();
            return map;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>       
        public IDictionary<string, byte> GetBuildingLevelMap()
        {
            //TODO: 暂未实现省份建筑
            var map = new Dictionary<string, byte>(8);

            if (TryGetHistoryNode(out var history) && history.Has(Key.Buildings))
            {
                var buildingsNode = history.Child(Key.Buildings).Value;
                foreach (var item in buildingsNode.Leaves)
                {
                    var value = byte.Parse(item.Value.ToRawString());
                    map.Add(item.Key, value);
                }
            }
            return map;
        }

        public string GetName()
        {
            if (!_state.Has(Key.Name))
            {
                return string.Empty;
            }
            var leafs = _state.Leafs(Key.Name);
            if (leafs.Count() != 1)
            {
                _logger.Warn("{0} 不唯一", Key.Name);
            }
            var nameLeaf = leafs.Last();

            return nameLeaf.Value.ToRawString();
        }
    }
}