using System.IO;
using System.Text;
using CWTools.CSharp;
using CWTools.Parser;
using CWTools.Process;
using FParsec;
using Microsoft.FSharp.Collections;
using static CWTools.Parser.CKParser;
using static CWTools.Process.CK2Process;

namespace HOI_Message.Logic.Util.CWTool;

/// <summary>
/// CWTools的适配器.
/// </summary>
public class CWToolsAdapter
{
    public Node Root { get; }
    public bool IsSuccess { get; }
    public ParserError Error { get; }
    public string ErrorMessage => Error.ErrorMessage;

    static CWToolsAdapter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// 根据文件路径解析文件
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public CWToolsAdapter(string filePath) 
        : this(Path.GetFileName(filePath), filePath, File.Exists(filePath) 
            ? Parsers.ParseScriptFile(Path.GetFileName(filePath), File.ReadAllText(filePath)) 
            : throw new FileNotFoundException($"{filePath} 不存在", filePath))
    {
    }

    private CWToolsAdapter(string fileName, string filePath, CharParsers.ParserResult<FSharpList<Types.Statement>, Microsoft.FSharp.Core.Unit> node)
    {
        Root = node.IsSuccess ? Parsers.ProcessStatements(fileName, filePath, node.GetResult()) : new Node(string.Empty);
        IsSuccess = node.IsSuccess;
        Error = node.GetError();
    }
}
