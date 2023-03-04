using NLog;
using System.Collections.Generic;
using System.IO;
using static HOI_Message.Logic.Localisation.LocalisationParser;

namespace HOI_Message.Logic.Localisation;

public sealed class GameLocalisation
{
    private readonly Dictionary<string, LineData> _datas;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public GameLocalisation()
    {
        _datas = new Dictionary<string, LineData>();
    }

    public void AddByFilePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath} 不存在");
        }
        var data = new LocalisationParser(filePath);
        AddToMap(data.AllData);
    }

    public void AddByMap(IDictionary<string, LineData> map)
    {
        AddToMap(map);
    }

    public void TrimExcess()
    {
        _datas.TrimExcess();
    }

    private void AddToMap(IDictionary<string, LineData> map)
    {
        foreach (var item in map)
        {
#if DEBUG
            if (item.Key != item.Value.Key)
            {
                _logger.Warn("错误的Key, 应为:'{0}', 结果:'{1}'", item.Key, item.Value.Key);
            }
#endif
            if (_datas.TryGetValue(item.Key, out var oldData))
            {
                if (item.Value.Level > oldData.Level)
                {
                    _datas[item.Key] = item.Value;
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
                _datas.Add(item.Key, item.Value);
            }
        }
    }

    public string GetValue(string key)
    {
        if (_datas.TryGetValue(key, out var lineData))
        {
            return lineData.Value;
        }
        else
        {
            return key;
        }
    }

    public string GetResourceName(string resourceType)
    {
        return GetValue($"PRODUCTION_MATERIALS_{resourceType.ToUpper()}");
    }

    public bool TryGetValue(string key, out string value)
    {
        if (_datas.TryGetValue(key, out var data))
        {
            value = data.Value;
            return true;
        }

        value = key;
        return false;
    }

    public string GetCountryName(CountryTag countryTag)
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

    public string GetCountryNameByRulingParty(CountryTag countryTag, string rulingParty)
    {
        if (TryGetValue($"{countryTag}_{rulingParty}", out var name))
        {
            return name;
        }
        return GetCountryName(countryTag);
    }
}
