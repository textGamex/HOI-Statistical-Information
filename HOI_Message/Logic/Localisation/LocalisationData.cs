using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace HOI_Message.Logic.Localisation;

// TODO: 太简陋了, 有时间写个解析器
// 因为 CWTools 的本地化解析器不支持中文解析, 所以只能自己写了.
public partial class LocalisationData
{
    public LanguageType Language { get; }
    public ReadOnlyDictionary<string, Data> AllData => _datas.AsReadOnly();
    private readonly Dictionary<string, Data> _datas;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public LocalisationData(string filePath)
    {
        var lines = new List<string>(File.ReadAllLines(filePath, new UTF8Encoding(true)));
        _ = lines.RemoveAll(string.IsNullOrEmpty);
        if (lines.Count == 0)
        {
            _datas = new Dictionary<string, Data>();
            Language = LanguageType.Unknown;
            return;
        }
        Language = GetLanguageType(lines[0].Trim());
        lines.RemoveAt(0);

        _datas = new(lines.Count);
        foreach (var line in lines)
        {
            var (Key, Value, Level) = Parse(line);
            if (_datas.TryGetValue(Key, out var oldData))
            {
                if (Level >= oldData.Level)
                {
                    _datas[Key] = new Data(Key, Value, Level);
                }
            }
            else
            {
                _datas.Add(Key, new Data(Key, Value, Level));
            }
        }
    }

    private static LanguageType GetLanguageType(string text)
    {
        return text switch
        {
            "l_english:" => LanguageType.English,
            _ => LanguageType.Unknown
        };
    }

    private static (string Key, string Value, byte Level) Parse(string line)
    {
        var result = LocalisationTextRegex().Match(line);
        if (!result.Success)
        {
            return (string.Empty, string.Empty, default);
        }

        return (result.Groups["Key"].Value, result.Groups["Value"].Value, byte.Parse(result.Groups["Level"].Value));
    }

    private static bool IsDescription(string key)
    {
        const string keyWords = "desc";

        var array = key.Split('_');
        if (array.Length != 0 && array[^1] == keyWords)
        {
            return true;
        }
        return false;
    }

    public class Data
    {
        public string Key { get; }
        public string Value { get; }
        public byte Level { get; }
        public static Data Empty => _empty;
        private static readonly Data _empty = new(string.Empty, string.Empty, default);

        public Data(string key, string value, byte level = 0)
        {
            Key = key;
            Value = value;
            Level = level;
        }

        protected bool Equals(Data other)
        {
            return Key == other.Key && Value == other.Value && Level == other.Level;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Data)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value, Level);
        }

        public static bool operator ==(Data? left, Data? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Data? left, Data? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{{Key='{Key}', Value='{Value}', Level={Level}}}";
        }
    }

    public enum LanguageType : byte
    {
        Unknown,
        English
    }

    [GeneratedRegex("(?<Key>\\S*):\\s*(?<Level>\\d)\\s*\"(?<Value>.*)\"", RegexOptions.Compiled)]
    private static partial Regex LocalisationTextRegex();
}
