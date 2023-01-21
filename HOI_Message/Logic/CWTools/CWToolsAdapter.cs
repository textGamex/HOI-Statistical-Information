using System;
using System.IO;
using System.Text;
using CWTools.CSharp;
using CWTools.Parser;
using FParsec;
using static CWTools.Parser.CKParser;
using static CWTools.Process.CK2Process;

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

    /// <summary>
    /// 根据路径解析文件
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public CWToolsAdapter(string filePath) 
        : this(File.Exists(filePath) ? parseEventFile(filePath) : throw new FileNotFoundException($"{filePath} 不存在", filePath))
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="text"></param>
    public CWToolsAdapter(string fileName, string text) 
        : this(parseEventString(text, fileName))
    {
    }

    private CWToolsAdapter(CharParsers.ParserResult<Types.ParsedFile, Microsoft.FSharp.Core.Unit> eventRoot)
    {
        if (eventRoot.IsSuccess)
        {
            Root = processEventFile(eventRoot.GetResult());
        }
        else
        {
            Root = new EventRoot(string.Empty, null);
        }
        IsSuccess = eventRoot.IsSuccess;
        ErrorMessage = eventRoot.GetError();
    }
}
