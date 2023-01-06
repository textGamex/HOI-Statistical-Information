using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using HandyControl.Data;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.UI;
using NLog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HOI_Message.UI.OptionView;
using System.Windows.Documents;
using System.Windows.Media;

namespace HOI_Message;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class SelectPathsWindow
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public SelectPathsWindow()
    {
        InitializeComponent();

        this.DataContext = new ViewModels.SelectPathsWindowViewModel();
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ParseProgressBar.Visibility = Visibility.Hidden;
        ParseProgressMesaageShowLabel.Visibility = Visibility.Hidden;
        ParseItemNameShowLabel.Visibility = Visibility.Hidden;
        ParseProgressMesaageShowLabel.Content = string.Empty;
       
        AddAllWeakReferenceMessenger();
    }

    private void AddAllWeakReferenceMessenger()
    {
        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.StartParseData, (_, _) =>
        {
            ParseProgressMesaageShowLabel.Visibility = Visibility.Visible;
            ParseProgressBar.Visibility = Visibility.Visible;
            ParseItemNameShowLabel.Visibility = Visibility.Visible;

            StartParseButton.Visibility = Visibility.Hidden;
        });

        WeakReferenceMessenger.Default.Register<string, byte>(this, EventId.ParseDataSuccess, (_, _) =>
        {
            ShowSuccessTipMessage();

            var newWindow = new OptionWindow();
            newWindow.Show();
            this.Close();

            //取消注册
            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        WeakReferenceMessenger.Default.Register<Tuple<double, FileInfo>, byte>(this, EventId.UpdateParseProgressBar, (_, data) =>
        {
            _ = Dispatcher.Invoke(new Action<DependencyProperty, object>(this.ParseProgressBar.SetValue),
                    DispatcherPriority.Background, ProgressBar.ValueProperty, data.Item1);
            ParseProgressMesaageShowLabel.Content = $"{data.Item2.FullName} 处理完成";
        });        
    }    
       
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var win = new OptionWindow();
        win.Show();
    }

    private void ShowSuccessTipMessage()
    {
        ParseProgressMesaageShowLabel.Content = "完成";
        var label = new Label() { Content = "完成", Width = 200, Height = 100, FontSize = 20 };
        _ = Notification.Show(label, ShowAnimation.VerticalMove);
    }
}
