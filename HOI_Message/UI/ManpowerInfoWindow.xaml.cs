using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using HOI_Message.ViewModels;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.UI;

/// <summary>
/// ManpowerInfo.xaml 的交互逻辑
/// </summary>
public partial class ManpowerInfoWindow
{
    public ManpowerInfoWindow()
    {
        InitializeComponent();

        this.DataContext = new ManpowerInfoWindowViewModel(GameModels.Countries, GameModels.Localisation);

        ManpowerPieChart.LegendTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            FontFamily = GlobalSettings.DefaultChartFont,
            IsFill = false,
        };

        ManpowerPieChart.TooltipTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            FontFamily = GlobalSettings.DefaultChartFont
        };        

        ManpowerPieChart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden;
    }
}
