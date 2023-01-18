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

namespace HOI_Message.UI.OptionView
{
    /// <summary>
    /// CountriesInfoOptionView.xaml 的交互逻辑
    /// </summary>
    public partial class CountriesInfoOptionView : UserControl
    {
        public CountriesInfoOptionView()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.Option.CountriesInfoViewModel();

            WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ShowCountriesInfoWindow, (_, _) =>
            {
                var window = new CountriesInfoWindow();
                window.Show();
            });
        }
    }
}
