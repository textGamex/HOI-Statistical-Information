using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

namespace HOI_Message.UI.OptionView
{
    /// <summary>
    /// StatesOptionView.xaml 的交互逻辑
    /// </summary>
    public partial class StatesOptionView : UserControl
    {
        public StatesOptionView()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.Option.StatesOptionViewModel();

            WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ShowStatesInfoWindow, (_, _) =>
            {
                var window = new StatesInfoWindow();
                window.Show();
            });
        }
    }
}
