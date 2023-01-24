using System.Windows.Controls;
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
