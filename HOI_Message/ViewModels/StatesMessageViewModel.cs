using CommunityToolkit.Mvvm.ComponentModel;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HOI_Message.ViewModels;

[ObservableObject]
public partial class StatesMessageViewModel
{
    [ObservableProperty]
    private string statesSum;
    public StatesMessageViewModel(IEnumerable<NationalInfo> countries, GameLocalisation localisation)
    {
        var data = new ObservableCollection<ISeries>();
        var smallCountries = new List<NationalInfo>();
        int totalStates = 0;

        foreach (var country in countries.OrderByDescending(x => x.StateInfos.Count()))
        {
            totalStates += country.OwnStatesNumber;

            if (country.OwnStatesNumber <= 10)
            {
                smallCountries.Add(country);
                continue;
            }

            var pieSeries = new PieSeries<NationalInfo>
            {
                Values = new []{ country },
                Name = $"{localisation.GetCountryNameByRulingParty(country.Tag, country.RulingParty)}: {country.OwnStatesNumber}",
                Mapping = (message, point) =>
                {
                    point.PrimaryValue = message.OwnStatesNumber;
                    point.SecondaryValue = point.Context.Entity.EntityIndex;
                }
            };

            if (country.OwnStatesNumber > 13)
            {
                pieSeries.DataLabelsSize = 10;
                pieSeries.DataLabelsPaint = new SolidColorPaint(SKColors.Black)
                {
                    FontFamily = GlobalSettings.DefaultChartFont
                };
                pieSeries.DataLabelsPosition = PolarLabelsPosition.Middle;
                pieSeries.DataLabelsFormatter = point => $"{localisation.GetCountryNameByRulingParty(country.Tag, country.RulingParty)}({point.StackedValue.Share:P2})";
                if (country.OwnStatesNumber > 30)
                {
                    pieSeries.DataLabelsSize = 22;
                }
                else if (country.OwnStatesNumber > 20)
                {
                    pieSeries.DataLabelsSize = 18;
                }
                else
                {
                    pieSeries.DataLabelsSize = 16;
                }
            }

            if (country.MapColor is not null)
            {
                pieSeries.Fill = new SolidColorPaint((SKColor)country.MapColor);
            }
            data.Add(pieSeries);
        }

        data.Add(new PieSeries<double>
        {                
            Values = new double[] { smallCountries.Sum(x => x.OwnStatesNumber) },
            Name = "其它",
            DataLabelsFormatter = point => $"其它 ({point.StackedValue.Share:P2})",
            DataLabelsSize = 25,
            DataLabelsPaint = new SolidColorPaint(SKColors.Black)
            {
                FontFamily = GlobalSettings.DefaultChartFont
            },
            DataLabelsPosition = PolarLabelsPosition.Middle,
        });
        Series = data;

        StatesSum = $"地区总数: {totalStates}";
    }
    public IEnumerable<ISeries> Series { get; set; }
}
