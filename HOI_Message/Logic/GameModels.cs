using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using HOI_Message.Logic.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI_Message.Logic;

public static class GameModels
{
    /// <summary>
    /// 生成图像时应该调用这个方法
    /// </summary>
    public static IReadOnlyCollection<NationalInfo> Countries => _countries.AsReadOnly();

    public static GameLocalisation Localisation { get; set; } = GameLocalisation.Empty;

#pragma warning disable IDE1006 // 命名样式
    public static IList<NationalInfo> _countries { get; internal set; } = new List<NationalInfo>();
#pragma warning restore IDE1006 // 命名样式
}
