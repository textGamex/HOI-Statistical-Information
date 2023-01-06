using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HOI_Message.ViewModels;

internal class ManpowerInfoWindowViewModel
{
    public ManpowerInfoWindowViewModel(IEnumerable<NationalInfo> countries, GameLocalisation localisation)
    {
        var data = new ObservableCollection<ISeries>();
        ushort count = 0;
        foreach (var country in countries.OrderByDescending(x => x.Manpower))
        {
            ++count;
            var pieSeries = new PieSeries<NationalInfo>
            {
                Values = new NationalInfo[] { country },
                Name = $"{count}. {localisation.GetCountryName(localisation.GetValue(country.Tag))}",

                Mapping = (message, point) =>
                {
                    point.PrimaryValue = message.Manpower;
                    point.SecondaryValue = point.Context.Entity.EntityIndex;
                },
                TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue} ({chartPoint.StackedValue.Share:P2})",
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
            data.Add(pieSeries);
        }
        Series = data;
    }
    public IEnumerable<ISeries> Series { get; set; }
}
