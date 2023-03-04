using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.ViewModels;

internal partial class ManpowerInfoWindowViewModel : ObservableObject
{
    [ObservableProperty] 
    private string totalManpower;

    public ManpowerInfoWindowViewModel(IEnumerable<NationalInfo> countries, GameLocalisation localisation)
    {
        var data = new ObservableCollection<ISeries>();
        ushort count = 0;
        long manpowerSum = 0;

        foreach (var country in countries.OrderByDescending(x => x.ManpowerSum))
        {
            if (country.ManpowerSum == 0)
            {
                continue;
            }

            ++count;
            manpowerSum += country.ManpowerSum;

            var pieSeries = new PieSeries<NationalInfo>
            {
                Values = new NationalInfo[] { country },
                Name = $"{count}. {localisation.GetCountryNameByRulingParty(country.Tag, country.RulingParty)}",

                Mapping = (message, point) =>
                {
                    point.PrimaryValue = message.ManpowerSum;
                    point.SecondaryValue = point.Context.Entity.EntityIndex;
                },
                TooltipLabelFormatter = (chartPoint) => 
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue} ({chartPoint.StackedValue.Share:P2})",
            };

            if (count < 10)
            {
                pieSeries.DataLabelsPosition = PolarLabelsPosition.Outer;
                pieSeries.DataLabelsFormatter = 
                    p => $"{GameModels.Localisation.GetCountryName(country.Tag)} ({p.StackedValue.Share:P2})";
                pieSeries.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30))
                {
                    FontFamily = GlobalSettings.DefaultChartFont
                };
            }

            if (country.MapColor is not null)
            {
                pieSeries.Fill = new SolidColorPaint((SKColor)country.MapColor);
            }
            data.Add(pieSeries);
        }
        Series = data;

        TotalManpower = $"全球总人口: {manpowerSum}";
    }
    public IEnumerable<ISeries> Series { get; set; }
}
