using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using System.Collections.Generic;

namespace HOI_Message.Logic;

public static class GameModels
{
    /// <summary>
    /// 平时应该调用这个方法
    /// </summary>
    public static IEnumerable<NationalInfo> Countries => _countries.AsReadOnly();

    public static GameLocalisation Localisation { get; set; } = new();

#pragma warning disable IDE1006 // 命名样式
    public static IList<NationalInfo> _countries { get; internal set; } = new List<NationalInfo>();
#pragma warning restore IDE1006 // 命名样式
}
