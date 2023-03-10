using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using HandyControl.Data;
using HOI_Message.Logic;
using HOI_Message.Logic.Util.CWTool;
using HOI_Message.UI.OptionView;
using NLog;

namespace HOI_Message;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class SelectPathsWindow
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private bool _isUserClose = true;

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

            _isUserClose = false;
            var newWindow = new OptionWindow();
            newWindow.Show();
            this.Close();

            //取消注册
            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        // double是进度, string是文件绝对路径
        WeakReferenceMessenger.Default.Register<Tuple<double, string>, byte>(this, EventId.UpdateParseProgressBar, (_, data) =>
        {
            _ = Dispatcher.Invoke(new Action<DependencyProperty, object>(this.ParseProgressBar.SetValue),
                    DispatcherPriority.Background, ProgressBar.ValueProperty, data.Item1);
            ParseProgressMesaageShowLabel.Content = $"{data.Item2} 处理完成";
        });
    }

    private void ShowSuccessTipMessage()
    {
        ParseProgressMesaageShowLabel.Content = "完成";
        var label = new Label() { Content = "完成", Width = 200, Height = 100, FontSize = 20 };
        _ = Notification.Show(label, ShowAnimation.VerticalMove);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        if (_isUserClose)
        {
            Environment.Exit(0);
        }
    }
}
