using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;

namespace HOI_Message.ViewModels.Option;

internal partial class ResourcesOptionViewModel : ObservableObject
{
    private readonly Dictionary<string, (string name, string desc)> _rescourcesNameMap = new(8);

    public ResourcesOptionViewModel()
    {
        foreach (var country in GameModels.Countries)
        {
            foreach (var type in country.GetAllResourcesTypes())
            {
                if (_rescourcesNameMap.ContainsKey(type))
                {
                    continue;
                }
                else
                {
                    var name = GameModels.Localisation.GetValue($"PRODUCTION_MATERIALS_{type.ToUpper()}");
                    var desc = GameModels.Localisation.GetValue($"{type}_desc");
                    _rescourcesNameMap[type] = (name, desc);
                }
            }
        }

        foreach (var (name, desc) in _rescourcesNameMap.Values)
        {
            comboBoxItems.Add(new ComboBoxItem() { Content = name, ToolTip = desc });
        }
    }

    [ObservableProperty]
    List<ComboBoxItem> comboBoxItems = new();

    [RelayCommand]
    private void ClickStatrButton(ComboBoxItem comboBoxItem)
    {
        var typeName = (string)comboBoxItem.Content;
        string? typeKey = null;

        foreach (var item in _rescourcesNameMap)
        {
            if (item.Value.name == typeName)
            {
                typeKey = item.Key;
                break;
            }
        }
        if (typeKey is null)
        {
            throw new ArgumentNullException();
        }

        WeakReferenceMessenger.Default.Send(typeKey, EventId.ShowResourcesInfoWindow);
    }
}
