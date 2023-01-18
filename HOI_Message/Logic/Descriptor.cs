using System.IO;
using System.Collections.Generic;
using System.Linq;
using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.Util.CWTool;

namespace HOI_Message.Logic
{
    public class Descriptor
    {
        public string Name { get; } = string.Empty;

        public string Version { get; } = string.Empty;

        public IEnumerable<string> Tags { get; } = Enumerable.Empty<string>();

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
                throw new ParseException($"无法解析文件, path: {modDescriptorPath}, 错误信息: {root.ErrorMessage}");
            }

            var result = root.Root.Leaves;

            foreach (var item in result)
            {
                if (item.Key == Key.Name)
                {
                    Name = item.Value.ToRawString();
                }

                if (item.Key == Key.Version)
                {
                    Version = item.Value.ToRawString();
                }

                if (item.Key == Key.Picture)
                {
                    PictureName = item.Value.ToRawString();
                }
            }

            if (root.Root.Has(Key.Tags))
            {
                var tags = root.Root.Child(Key.Tags).Value;
                Tags = tags.LeafValues.Select(x => x.Key);
            }
        }

        private static class Key
        {
            public const string Name = "name";
            public const string Version = "version";
            public const string Picture = "picture";
            public const string Tags = "tags";
        }
    }
}
