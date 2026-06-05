using System.Text;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sts2YamlLoc.IO;

public sealed class YamlLocBundleReader(string rootPath) : ILocBundleProducer<NestedEntry>
{
    public LocBundle<NestedEntry> Produce()
    {
        var groups = new Dictionary<string, LocTableGroup<NestedEntry>>();

        if (!Directory.Exists(rootPath))
            return new LocBundle<NestedEntry>(groups);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        foreach (var langDir in Directory.EnumerateDirectories(rootPath))
        {
            var language = Path.GetFileName(langDir);
            var tables = new Dictionary<string, LocTable<NestedEntry>>();

            foreach (var file in Directory.EnumerateFiles(langDir, "*.yaml"))
            {
                var tableName = Path.GetFileNameWithoutExtension(file);
                using var sr = new StreamReader(file, Encoding.UTF8);
                var obj = deserializer.Deserialize(sr);

                IList<NestedEntry> entries = new List<NestedEntry>();
                if (obj is IDictionary<object, object> dict)
                {
                    entries = StructureHelper.ExtractEntries(dict);
                }

                tables[tableName] = new LocTable<NestedEntry>(entries);
            }

            groups[language] = new LocTableGroup<NestedEntry>(tables);
        }

        return new LocBundle<NestedEntry>(groups);
    }
}
