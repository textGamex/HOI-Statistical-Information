using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

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
