using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI_Message.ViewModels.Option
{
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
    }
}
