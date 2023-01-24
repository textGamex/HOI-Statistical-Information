using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using HOI_Message.Logic;

namespace HOI_Message.ViewModels;

internal partial class CountriesInfoWindowViewModel : ObservableObject
{
	public CountriesInfoWindowViewModel() 
	{
		var localisation = GameModels.Localisation;
		foreach (var country in GameModels.Countries)
		{
			datas.Add(new CountryData(localisation.GetCountryNameByRulingParty(country.Tag, country.RulingParty),
				country.ResearchSlotsNumber,
				localisation.GetValue(country.RulingParty),
				country.ArmyUnitInfo.UnitSum));
		}
	}

	[ObservableProperty]
	private List<CountryData> datas = new();

	public class CountryData
	{
		public string Name { get; }
		public byte ResearchSlotsNumber { get; }
		public string RulingParty { get; }
		public int DivisionsSum { get; }

		public CountryData(string name, byte researchSlotsNumber, string rulingParty, int divisionsSum)
		{
			Name = name;
			ResearchSlotsNumber = researchSlotsNumber;
			RulingParty = rulingParty;
			DivisionsSum = divisionsSum;
		}
	}
}
