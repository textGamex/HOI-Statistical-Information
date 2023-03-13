using System.IO;
using System.Collections.Generic;
using System.Linq;
using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.Util.CWTool;
using System.Text.RegularExpressions;

namespace HOI_Message.Logic;

/// <summary>
/// MOD描述文件类
/// </summary>
public partial class Descriptor
{
    public string Name { get; private set; } = string.Empty;

    public string SupportedVersion { get; } = string.Empty;
    public string Version { get; } = string.Empty;

    public IEnumerable<string> Tags { get; }

    public string PictureName { get; } = string.Empty;

    /// <summary>
    /// 按文件绝对路径构建
    /// </summary>
    /// <param name="modDescriptorPath">mod描述文件绝对路径</param>
    /// <exception cref="ParseException">当文件解析失败时</exception>
    /// <exception cref="FileNotFoundException">当文件不存在时</exception>
    public Descriptor(string modDescriptorPath)
    {
        var root = new CWToolsAdapter(modDescriptorPath);
        if (!root.IsSuccess)
        {
            throw new ParseException($"无法解析 descriptor.mod 文件, 错误信息: {root.ErrorMessage}");
        }

        var result = root.Root.Leaves;

        foreach (var item in result)
        {
            switch (item.Key)
            {
                case Key.Name:
                    Name = item.ValueText;
                    break;
                case Key.SupportedVersion:
                    SupportedVersion = item.ValueText;
                    break;
                case Key.Picture:
                    PictureName = item.ValueText;
                    break;
                case Key.Version:
                    Version = item.ValueText;
                    break;
            }
        }

        if (root.Root.Has(Key.Tags))
        {
            var tags = root.Root.Child(Key.Tags).Value;
            Tags = tags.LeafValues.Select(x => x.Key);
        }
        else
        {
            Tags = Enumerable.Empty<string>();
        }
    }
    
    private static class Key
    {
        public const string Name = "name";
        public const string SupportedVersion = "supported_version";
        public const string Version = "version";
        public const string Picture = "picture";
        public const string Tags = "tags";
    }
}