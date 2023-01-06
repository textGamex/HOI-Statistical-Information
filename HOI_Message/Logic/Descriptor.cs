using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random_HOI4.Logic.Util.CWTool;
using SkiaSharp;

namespace HOI_Message.Logic
{
    public class Descriptor
    {
        public string Name { get; } = string.Empty;

        public string Version { get; } = string.Empty;

        public IEnumerable<string> Tags { get; } = Enumerable.Empty<string>();

        public string Picture { get; } = string.Empty; 

        public Descriptor(string path)
        {
            if (!CWToolsAdapter.TryParseFile(path, out var root))
            {
                throw new ArgumentException($"无法解析文件, path: {path}", nameof(path));
            }

            var result = root.Root.Leaves;

            foreach (var item in result)
            {
                if (item.Key == Key.Name)
                {
                    Name = item.Value.ToString();
                }

                if (item.Key == Key.Version)
                {
                    Version = item.Value.ToString();
                }

                if (item.Key == Key.Picture)
                {
                    Picture = item.Value.ToString();
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
