using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;
using NLog;

namespace HOI_Message.UI.OptionView;

[ObservableObject]
/// <summary>
/// OptionWindow.xaml 的交互逻辑
/// </summary>
public partial class OptionWindow
{    
    [ObservableProperty]
    List<Label> menus = new();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public OptionWindow()
    {
        InitializeComponent();

        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.DataContext = new ViewModels.Option.OptionWindowViewModel();

        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ClickMenuResourcesOption, (_, _) =>
        {
            this.MainMessageShowGrid.Children.Clear();
            _ = this.MainMessageShowGrid.Children.Add(new ResourcesOptionView());
        });

        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ClickMenuManpowerOption, (_, _) =>
        {
            this.MainMessageShowGrid.Children.Clear();
            _ = this.MainMessageShowGrid.Children.Add(new ManpowerOptionView());
        });

        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ClickMenuStatesOption, (_, _) =>
        {
            this.MainMessageShowGrid.Children.Clear();
            _ = this.MainMessageShowGrid.Children.Add(new StatesOptionView());
        });
    }
}
