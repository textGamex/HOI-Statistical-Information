using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

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
