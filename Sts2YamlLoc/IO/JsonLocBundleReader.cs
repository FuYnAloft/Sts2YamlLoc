using System.Text;
using System.Text.Json;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.IO;

public sealed class JsonLocBundleReader(string rootPath) : ILocBundleProducer<FlatEntry>
{
    public LocBundle<FlatEntry> Produce()
    {
        var groups = new Dictionary<string, LocTableGroup<FlatEntry>>();

        if (!Directory.Exists(rootPath))
            return new LocBundle<FlatEntry>(groups);

        foreach (var langDir in Directory.EnumerateDirectories(rootPath))
        {
            var language = Path.GetFileName(langDir);
            var tables = new Dictionary<string, LocTable<FlatEntry>>();

            foreach (var file in Directory.EnumerateFiles(langDir, "*.json"))
            {
                var tableName = Path.GetFileNameWithoutExtension(file);
                var text = File.ReadAllText(file, Encoding.UTF8);
                Dictionary<string, string>? dict;
                try
                {
                    dict = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                }
                catch
                {
                    dict = new Dictionary<string, string>();
                }

                var entries = dict!.Select(kv => new FlatEntry(kv.Key, kv.Value)).ToList();
                tables[tableName] = new LocTable<FlatEntry>(entries);
            }

            groups[language] = new LocTableGroup<FlatEntry>(tables);
        }

        return new LocBundle<FlatEntry>(groups);
    }
}
