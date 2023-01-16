using System;
using System.Linq;
using HOI_Message.Logic.Util.CWTool;

namespace HOI_Message.Logic.Country;

public class CountryFileParser
{
    public string OOBName { get; private set; } = string.Empty;

    /// <summary>
    /// 如果国家文件里没有设置则默认为3个科研槽
    /// </summary>
    public byte ResearchSlotsNumber { get; private set; } = 3;
    public int CapitalId { get; private set; } = -1;
    public int ConvoysNumber { get; private set; } = 0;

    public static bool TryParseFile(string filePath, out CountryFileParser? parser, out string? errorMessage)
    {
        var adapter = new CWToolsAdapter(filePath);
        if (adapter.IsSuccess)
        {
            parser = Parse(adapter);
            errorMessage = string.Empty;
            return true;
        }        
        parser = null;
        errorMessage = adapter.ErrorMessage;
        return false;
    }

    private static CountryFileParser Parse(CWToolsAdapter adapter)
    {
        var root = adapter.Root;
        var parser = new CountryFileParser();

        if (root.Has(Key.OOB))
        {
            var oobName = root.Leafs(Key.OOB).Last().Value;
            parser.OOBName = oobName.ToRawString();
        }

        if (root.Has(Key.SetResearchSlots))
        {
            var slotsNumber = root.Leafs(Key.SetResearchSlots).Last().Value;
            parser.ResearchSlotsNumber = byte.Parse(slotsNumber.ToString());
        }

        if (root.Has(Key.Capital))
        {
            var capital = root.Leafs(Key.Capital).Last().Value;
            parser.CapitalId = int.Parse(capital.ToString());
        }

        if (root.Has(Key.SetConvoys))
        {
            var number = root.Leafs(Key.SetConvoys).Last().Value;
            parser.ConvoysNumber = int.Parse(number.ToString());
        }
        return parser;
    }

    private CountryFileParser() { }

    private static class Key
    {
        public const string OOB = "oob";
        public const string SetResearchSlots = "set_research_slots";
        public const string Capital = "capital";
        public const string SetConvoys = "set_convoys";
    }
}



