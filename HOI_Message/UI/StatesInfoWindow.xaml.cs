using HOI_Message.Logic.Country;
using HOI_Message.ViewModels;
using HOI_Message.Logic.Localisation;
using System.Collections.Generic;
using System.Windows;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using HOI_Message.Logic;

namespace HOI_Message.UI;

/// <summary>
/// StatesMessage.xaml 的交互逻辑
/// </summary>
///     
public partial class StatesInfoWindow
{
    public StatesInfoWindow()
    {
        InitializeComponent();

        this.DataContext = new StatesMessageViewModel(GameModels.Countries, GameModels.Localisation);

        StatesPieChart.LegendTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            FontFamily = GlobalSettings.DefaultChartFont
        };

        StatesPieChart.TooltipTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,            
            FontFamily = GlobalSettings.DefaultChartFont
        };

        StatesPieChart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden;
    }
}
