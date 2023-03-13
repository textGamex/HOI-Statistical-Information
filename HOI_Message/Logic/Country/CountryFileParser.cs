using System.IO;
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
    public string RulingParty { get; private set; } = string.Empty;

    /// <summary>
    /// 尝试解析国家信息文件
    /// </summary>
    /// <remarks>
    /// 国家信息文件在 Hearts of Iron IV\history\countries 下
    /// </remarks>
    /// <param name="filePath">文件绝对路径</param>
    /// <param name="parser">解析器</param>
    /// <param name="errorMessage">错误信息</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public static bool TryParseFile(string filePath, out CountryFileParser? parser, out string errorMessage)
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

        parser.OOBName = TryGetOOBName(root);

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

        if (root.Has(Key.SetPolitics))
        {
            var politics = root.Child(Key.SetPolitics).Value;
            if (politics.Has(Key.RulingParty))
            {
                var rulingParty = politics.Leafs(Key.RulingParty).Last();
                parser.RulingParty = rulingParty.Value.ToRawString();
            }
        }
        return parser;
    }

    private CountryFileParser()
    {
    }

    private static string TryGetOOBName(CWTools.Process.Node root)
    {
        if (root.Has(Key.OOB))
        {
            var oobName = root.Leafs(Key.OOB).Last().Value;
            return oobName.ToRawString();
        }

        //TODO: 应该实现按DLC分类
        foreach (var node in root.Childs("if"))
        {
            if (node.Has("set_oob"))
            {
                return node.Leafs("set_oob").Last().Value.ToRawString();
            }
        }
        return string.Empty;
    }

    private static class Key
    {
        public const string OOB = "oob";
        public const string SetResearchSlots = "set_research_slots";
        public const string Capital = "capital";
        public const string SetConvoys = "set_convoys";
        public const string SetPolitics = "set_politics";
        public const string RulingParty = "ruling_party";
    }
}



