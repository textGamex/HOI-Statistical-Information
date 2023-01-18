using System.IO;
using System.Text;
using CWTools.CSharp;
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
    /// 根据路径解析文件.
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
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
