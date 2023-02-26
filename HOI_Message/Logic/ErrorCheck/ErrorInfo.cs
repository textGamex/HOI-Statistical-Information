namespace HOI_Message.Logic.ErrorCheck;

public class ErrorInfo
{
    private string _filePath;
    public string Message { get; }
    public uint LinesCount { get; }
    public ErrorTypes ErrorTypes { get; }
}