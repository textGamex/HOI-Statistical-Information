using HOI_Message.ViewModels;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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
