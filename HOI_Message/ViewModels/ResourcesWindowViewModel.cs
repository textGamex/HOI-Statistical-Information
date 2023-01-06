using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace HOI_Message.ViewModels;

public class ResourcesWindowViewModel
{
    public ResourcesWindowViewModel(string resourcesType, IEnumerable<NationalInfo> countries, GameLocalisation localisation)
    {
        var data = new ObservableCollection<ISeries>();
        uint count = 0;
        foreach (var country in countries.OrderByDescending(x => x.GetResourcesSum(resourcesType)))
        {
            ++count;
            var pieSeries = new PieSeries<NationalInfo>
            {
                Values = new NationalInfo[] { country },
                Name = $"{count}. {localisation.GetCountryName(country.Tag)}: {country.GetResourcesSum(resourcesType)}",
                Mapping = (message, point) =>
                {
                    point.PrimaryValue = message.GetResourcesSum(resourcesType);
                    point.SecondaryValue = point.Context.Entity.EntityIndex;
                }
            };
            data.Add(pieSeries);
        }
        Series = data;
    }

    public IEnumerable<ISeries> Series { get; set; }
}
