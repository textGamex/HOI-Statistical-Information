using System;
using System.Text;

namespace HOI_Message.Logic;

public readonly struct CountryTag : IEquatable<CountryTag>
{
    public static readonly CountryTag Empty = new(new string(' ', 3));
    private readonly byte _first;
    private readonly byte _second;
    private readonly byte _last;

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

        _first = (byte)tag[0];
        _second = (byte)tag[1];
        _last = (byte)tag[2];
    }

    public string Tag => ASCIIEncoding.ASCII.GetString(new []{ _first, _second, _last });

    public override string ToString()
    {
        return Tag;
    }

    public static implicit operator string(CountryTag tag)
    {
        return tag.Tag;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_first, _second, _last);
    }

    public bool Equals(CountryTag other)
    {
        return Equal(other);
    }

    private bool Equal(object? obj)
    {
        if (obj is not CountryTag tag)
            return false;
        return tag._first == _first && 
               tag._second == _second && 
               tag._last == _last;
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