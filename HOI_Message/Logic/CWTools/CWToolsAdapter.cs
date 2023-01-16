using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CWTools.Parser.CKParser;
using static CWTools.Process.CK2Process;
using static CWTools.Parser.Types;
using static FParsec.Error;
using CWTools.CSharp;
using CWTools.Process;
using CWTools.Parser;
using Microsoft.VisualBasic;

namespace HOI_Message.Logic.Util.CWTool;

/// <summary>
/// CWTools的适配器.
/// </summary>
internal class CWToolsAdapter
{
    public EventRoot Root { get; }
    public bool IsSuccess { get; }

    public string ErrorMessage { get; }

    static CWToolsAdapter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public CWToolsAdapter(string filePath)
    {
        var result = parseEventFile(filePath);
        if (result.IsSuccess)
        {
            Root = processEventFile(result.GetResult());
        }
        else
        {
            Root = new EventRoot(string.Empty, null);
        }
        IsSuccess = result.IsSuccess;
        ErrorMessage = result.GetError();
    }
}
