namespace HOI_Message.Logic.ErrorCheck;

public class ErrorInfo
{
    public string Message { get; }
    public uint Line { get; }
    public ErrorTypes Type { get; }

    private string _filePath;
}