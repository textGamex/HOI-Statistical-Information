namespace HOI_Message.UI
{
    /// <summary>
    /// CountriesInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CountriesInfoWindow
    {
        public CountriesInfoWindow()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.CountriesInfoWindowViewModel();
        }
    }
}
