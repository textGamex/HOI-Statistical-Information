using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.ViewModels;

public partial class ResourcesWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string totalResources;

    public ResourcesWindowViewModel(string resourcesType, IEnumerable<NationalInfo> countries, GameLocalisation localisation)
    {
        var data = new ObservableCollection<ISeries>();
        uint count = 0;
        long resourceSum = 0;

        foreach (var country in countries.OrderByDescending(x => x.GetResourcesSum(resourcesType)))
        {
            if (country.GetResourcesSum(resourcesType) == 0)
            {
                continue;
            }

            ++count;
            resourceSum += country.GetResourcesSum(resourcesType);

            var pieSeries = new PieSeries<NationalInfo>
            {
                Values = new []{ country },
                Name = $"{count}. {localisation.GetCountryNameByRulingParty(country.Tag, country.RulingParty)}",
                Mapping = (message, point) =>
                {
                    point.PrimaryValue = message.GetResourcesSum(resourcesType);
                    point.SecondaryValue = point.Context.Entity.EntityIndex;
                },                
                TooltipLabelFormatter = (chartPoint) => 
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue} ({chartPoint.StackedValue.Share:P2})",
            };

            if (country.MapColor is not null)
            {
                pieSeries.Fill = new SolidColorPaint((SKColor)country.MapColor);
            }

            data.Add(pieSeries);
            TotalResources = $"资源类型: {localisation.GetResourceName(resourcesType)}, 全球资源总数: {resourceSum}";
        }
        Series = data;
    }

    public IEnumerable<ISeries> Series { get; set; }
}
