using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

namespace HOI_Message.ViewModels.Option;

internal partial class CountriesInfoViewModel : ObservableObject
{
    [RelayCommand]
    private void ClickStartButton()
    {
        WeakReferenceMessenger.Default.Send(string.Empty, EventId.ShowCountriesInfoWindow);
    }
}
