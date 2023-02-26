using System;

namespace HOI_Message.Logic;

public readonly struct CountryTag : IEquatable<CountryTag>
{
    public static readonly CountryTag Empty = new(new string(' ', 3));
    private readonly char[] _tagsChars;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <exception cref="ArgumentException">不合规的Tag</exception>
    public CountryTag(string tag)
    {
        if (tag == string.Empty)
        {
            throw new ArgumentException("禁止空字符串");
        }
        if (tag.Length != 3)
        {
            throw new ArgumentException($"Tag不合规, '{tag}' 长度不等于 3");
        }

        _tagsChars = new char[3];
        _tagsChars[0] = tag[0];
        _tagsChars[1] = tag[1];
        _tagsChars[2] = tag[2];
    }

    public string Tag => new(_tagsChars);

    public override string ToString()
    {
        return Tag;
    }

    public static implicit operator string(CountryTag tag)
    {
        return tag.Tag;
    }

    public static implicit operator CountryTag(string tag)
    {
        return new CountryTag(tag);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tagsChars[0], _tagsChars[1], _tagsChars[2]);
    }

    public bool Equals(CountryTag other)
    {
        return Equal(other);
    }

    private bool Equal(object? obj)
    {
        if (obj is not CountryTag tag)
            return false;
        return tag._tagsChars[0] == _tagsChars[0] && 
               tag._tagsChars[1] == _tagsChars[1] && 
               tag._tagsChars[2] == _tagsChars[2];
    }

    public override bool Equals(object? obj)
    {
        return Equal(obj);
    }

    public static bool operator ==(CountryTag left, CountryTag right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CountryTag left, CountryTag right)
    {
        return !(left == right);
    }
}   