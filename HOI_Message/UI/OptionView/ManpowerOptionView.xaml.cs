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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.UI.OptionView;

/// <summary>
/// ManpowerOptionView.xaml 的交互逻辑
/// </summary>
public partial class ManpowerOptionView : UserControl
{
    public ManpowerOptionView()
    {
        InitializeComponent();

        this.DataContext = new ViewModels.Option.ManpowerOptionViewModel();

        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ShowManpowerInfoWindow, (_, _) =>
        {
            var window = new ManpowerInfoWindow();
            window.Show();
        });        
    }
}
