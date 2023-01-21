using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HOI_Message.Logic.Localisation.LocalisationData;

namespace HOI_Message.Logic.Localisation;

public sealed class GameLocalisation
{
    private readonly IDictionary<string, Data> _datas = new Dictionary<string, Data>();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public static GameLocalisation Empty => _empty;
    private readonly static GameLocalisation _empty = new();

    private GameLocalisation()
    {

    }

    //public GameLocalisation(string folderPath)
    //{
    //    var files = new DirectoryInfo(folderPath).GetFiles(".", SearchOption.AllDirectories);
    //    _datas = GetDatas(files.Select(x => x.FullName));
    //}

    public GameLocalisation(IDictionary<string, Data> datas)
    {
        _datas = datas;
    }

    public static void AddDataToMap(IDictionary<string, Data> map, string filePath)
    {
        var data = new LocalisationData(filePath);
        foreach (var item in data.AllData)
        {
#if DEBUG
            if (item.Key != item.Value.Key)
            {
                _logger.Warn("错误的Key, 应为:'{0}', 结果:'{1}'", item.Key, item.Value.Key);
            }
#endif
            if (map.TryGetValue(item.Key, out var oldData))
            {
                if (item.Value.Level > oldData.Level)
                {
                    map[item.Key] = item.Value;
                }
                else if (item.Value.Level == oldData.Level)
                {
                    _logger.Warn("两个具有相同 Key 的本地化值具有相同的优先级, data1:'{0}', data2:'{1}'", item, oldData);
                }
                else
                {
                    continue;
                }
            }
            else
            {
                map.Add(item.Key, item.Value);
            }
        }
    }

    public string GetValue(string key)
    {
        if (_datas.TryGetValue(key, out var value))
        {
            return value.Value;
        }
        else
        {
            return key;
        }
    }

    public bool TryGetValue(string key, out string value)
    {
        if (_datas.TryGetValue(key, out var data))
        {
            value = data.Value;
            return true;
        }
        else
        {
            value = key;
            return false;
        }
    }

    public string GetCountryName(string countryTag)
    {
        if (TryGetValue(countryTag, out var name1))
        {
            return name1;
        }
        else if (TryGetValue(countryTag + "_fascism_ADJ", out var name2))
        {
            return name2;
        }
        else if (TryGetValue(countryTag + "_democratic_ADJ", out var name3))
        {
            return name3;
        }
        else if (TryGetValue(countryTag + "_neutrality_ADJ", out var name4))
        {
            return name4;
        }
        else
        {
            return countryTag;
        }
    }

    public string GetCountryNameByRulingParty(string countryTag, string rulingParty)
    {
        if (TryGetValue($"{countryTag}_{rulingParty}", out var name))
        {
            return name;
        }
        return countryTag;
    }
}
