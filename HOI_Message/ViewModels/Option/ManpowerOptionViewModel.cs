using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

namespace HOI_Message.ViewModels.Option;

public partial class ManpowerOptionViewModel : ObservableObject
{
    [RelayCommand]
    private void ClickStatrButton()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ShowManpowerInfoWindow);
    }
}
