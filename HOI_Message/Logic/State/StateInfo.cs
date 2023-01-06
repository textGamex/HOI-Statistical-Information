using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HOI_Message.Logic.State;

public class StateInfo
{
    public string OwnerTag { get; }
    public int Id { get; }
    public string Name { get; }
    public uint Manpower { get; }
    public IReadOnlyDictionary<string, byte> Buildings => _buildingMap.AsReadOnly();
    public IReadOnlyDictionary<string, ushort> Resources => _resourcesMap.AsReadOnly();

    private readonly IList<string> _hasCoreTags;
    private readonly IDictionary<string, ushort> _resourcesMap;
    private readonly IDictionary<string, byte> _buildingMap;
    private readonly Lazy<int> _hashCode;

    public StateInfo(string path)
    {
        var parser = new StateFileParser(path);

        Id = parser.GetId();
        Name = parser.GetName();
        OwnerTag = parser.GetOwnerTag();
        Manpower = parser.GetManpower();
        _hasCoreTags = parser.GetHasCoreCountryTags();
        _resourcesMap = parser.GetResourcesMap();
        _buildingMap = parser.GetBuildingLevelMap();

        _hashCode = new Lazy<int>(GetHashCodeLazy);
    }

    public ReadOnlyCollection<string> GetHasCoreTags()
    {
        return _hasCoreTags.AsReadOnly();
    }

    #region 散列码实现
    public override int GetHashCode()
    {
        return _hashCode.Value;
    }

    private int GetHashCodeLazy()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(OwnerTag);
        hash.Add(Manpower);
        hash.Add(GetHasCoreTagsHashCode());
        hash.Add(GetMapHashCode(_resourcesMap));
        hash.Add(GetMapHashCode(_buildingMap));

        return hash.ToHashCode();
    }

    /// <summary>
    /// 获得<c>_hasCoreTags</c>变量的哈希值
    /// </summary>
    /// <returns>哈希值, 如果<c>_hasCoreTags</c>为空, 返回 <c>0</c></returns>
    private int GetHasCoreTagsHashCode()
    {
        int hash = 0;
        foreach (var item in _hasCoreTags)
        {
            hash = hash * 31 + item?.GetHashCode() ?? 0;
        }
        return hash;
    }

    private static int GetMapHashCode<TKet, TValue>(IDictionary<TKet, TValue> map)
    {
        int hashCode = 0;
        foreach (var key in map.Keys)
        {
            hashCode += (key == null ? 0 : key.GetHashCode()) ^
            (map[key] == null ? 0 : map[key].GetHashCode());
        }
        return hashCode;
    }
    #endregion
}
