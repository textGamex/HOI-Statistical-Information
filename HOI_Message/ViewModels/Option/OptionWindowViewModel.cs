using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

namespace HOI_Message.ViewModels.Option;

internal partial class OptionWindowViewModel : ObservableObject
{
    [RelayCommand]
    private void ClickMenuResourcesOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickMenuResourcesOption);
    }

    [RelayCommand]
    private void ClickMenuManpowerOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickMenuManpowerOption);
    }

    [RelayCommand]
    private void ClickMenuStatesOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickMenuStatesOption);
    }

    [RelayCommand]
    private void ClickMenuCountriesInfoOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickMenuCountriesInfoOption);
    }

    [RelayCommand]
    private void ClickMenuAppInfoOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickAppInfoOption);
    }

    [RelayCommand]
    private void ClickMenuErrorCheckOption()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ClickErrorCheckOption);
    }
}