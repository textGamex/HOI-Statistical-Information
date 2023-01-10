using System;
using System.Collections.Generic;
using System.Linq;
using Random_HOI4.Logic.Util.CWTool;

namespace HOI_Message.Logic
{
    public class Descriptor
    {
        public string Name { get; } = string.Empty;

        public string Version { get; } = string.Empty;

        public IEnumerable<string> Tags { get; } = Enumerable.Empty<string>();

        public string PictureName { get; } = string.Empty;

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
