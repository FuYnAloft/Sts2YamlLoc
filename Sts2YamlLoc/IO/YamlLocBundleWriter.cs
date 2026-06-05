using System.Text;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core.Events;

namespace Sts2YamlLoc.IO;

public sealed class YamlLocBundleWriter(string rootPath) : ILocBundleConsumer<NestedEntry>
{
    public void Consume(LocBundle<NestedEntry> table)
    {
        var serializer = new SerializerBuilder()
            .WithTypeConverter(new LiteralStringTypeConverter())
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        foreach (var (language, group) in table)
        {
            var langDir = Path.Combine(rootPath, language);
            Directory.CreateDirectory(langDir);

            foreach (var (tableName, locTable) in group)
            {
                var mapping = BuildNestedMapping(locTable);
                var text = serializer.Serialize(mapping);
                File.WriteAllText(Path.Combine(langDir, tableName + ".yaml"), text, Encoding.UTF8);
            }
        }
    }

    public static object BuildNestedMapping(IEnumerable<NestedEntry> entries)
    {
        var root = new Dictionary<string, object>();

        foreach (var entry in entries)
        {
            if (entry.Key.Count == 0)
                continue;

            IDictionary<string, object> current = root;
            for (var i = 0; i < entry.Key.Count - 1; i++)
            {
                var part = entry.Key[i];
                if (!current.TryGetValue(part, out var next))
                {
                    var d = new Dictionary<string, object>();
                    current[part] = d;
                    current = d;
                }
                else if (next is IDictionary<string, object> dict)
                {
                    current = dict;
                }
                else
                {
                    // conflict: existing scalar where we need mapping - overwrite with mapping
                    var d = new Dictionary<string, object>();
                    current[part] = d;
                    current = d;
                }
            }

            var leaf = entry.Key[^1];
            current[leaf] = entry.Value;
        }

        return root;
    }


    private sealed class LiteralStringTypeConverter : IYamlTypeConverter
    {
        private static readonly char[] SpecialStart =
            ['[', '{', '-', '#', '*', '&', '!', '>', '<', '%', '@', '`', '"', '\''];

        public bool Accepts(Type type) => type == typeof(string);

        public object? ReadYaml(IParser parser, Type type,
            ObjectDeserializer nestedObjectDeserializer)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                return scalar.Value;
            }

            return nestedObjectDeserializer(type);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type,
            ObjectSerializer nestedObjectSerializer)
        {
            var s = value?.ToString() ?? string.Empty;
            var trimmed = s.TrimStart();
            var useLiteral = s.Contains('\n') || (trimmed.Length > 0 && SpecialStart.Contains(trimmed[0]));

            var style = useLiteral ? ScalarStyle.Literal : ScalarStyle.Any;
            emitter.Emit(new Scalar(null, null, s, style, true, false));
        }
    }
}
