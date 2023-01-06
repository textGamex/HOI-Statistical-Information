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
using HOI_Message.ViewModels;
using HOI_Message.Logic.Localisation;
using HOI_Message.Logic.Country;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.IO;
using HOI_Message.Logic;

namespace HOI_Message.UI;

/// <summary>
/// ResourcesWindow.xaml 的交互逻辑
/// </summary>
public partial class ResourcesInfoWindow
{
    public ResourcesInfoWindow(string resourcesType)
    {
        InitializeComponent();

        this.DataContext = new ResourcesWindowViewModel(resourcesType, GameModels.Countries, GameModels.Localisation);
        
        ResourcesPieChart.LegendTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            FontFamily = GlobalSettings.DefaultChartFont
        };
        
        ResourcesPieChart.TooltipTextPaint = new SolidColorPaint 
        { 
            Color = SKColors.Black,          
            FontFamily = GlobalSettings.DefaultChartFont
        };

        ResourcesPieChart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden;
    }
}
