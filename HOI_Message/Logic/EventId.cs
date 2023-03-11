namespace HOI_Message.Logic;

/// <summary>
/// 事件ID, 用于 View 层与 View Model 层通信
/// </summary>
public static class EventId
{
    /// <summary>
    /// 开始解析时触发
    /// </summary>
    public const byte StartParseData = 0;

    /// <summary>
    /// 解析成功时触发
    /// </summary>
    public const byte ParseDataSuccess = 1;

    /// <summary>
    /// 在应该更新解析进度条时触发
    /// </summary>
    public const byte UpdateParseProgressBar = 2;

    /// <summary>
    /// 当用户点击菜单中的资源选项时触发
    /// </summary>
    public const byte ClickMenuResourcesOption = 3;

    /// <summary>
    /// 当用户点击菜单选项中的人力时触发
    /// </summary>
    public const byte ClickMenuManpowerOption = 4;

    /// <summary>
    /// 当用户点击菜单选项中的地区时触发
    /// </summary>
    public const byte ClickMenuStatesOption = 5;

    /// <summary>
    /// 显示资源信息
    /// </summary>
    public const byte ShowResourcesInfoWindow = 6;

    /// <summary>
    /// 显示人力信息
    /// </summary>
    public const byte ShowManpowerInfoWindow = 7;

    /// <summary>
    /// 显示地区信息
    /// </summary>
    public const byte ShowStatesInfoWindow = 8;

    /// <summary>
    /// 当用户点击菜单选项中的国家信息时触发
    /// </summary>
    public const byte ClickMenuCountriesInfoOption = 9;

    public const byte ShowCountriesInfoWindow = 10;

    /// <summary>
    /// 点击应用信息选项时触发
    /// </summary>
    public const byte ClickAppInfoOption = 11;

    /// <summary>
    /// 点击错误分析选项时触发
    /// </summary>
    public const byte ClickErrorCheckOption = 12;
}