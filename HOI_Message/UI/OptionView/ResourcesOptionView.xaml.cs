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
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.UI.OptionView
{
    /// <summary>
    /// ResourcesOptionView.xaml 的交互逻辑
    /// </summary>
    public partial class ResourcesOptionView : UserControl
    {
        public ResourcesOptionView()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.Option.ResourcesOptionViewModel();

            WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ShowResourcesInfoWindow, (o, resourcesType) =>
            {
                var window = new ResourcesInfoWindow(resourcesType);
                window.Show();
            });            
        }
    }
}
