using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.IO;

public sealed class JsonLocBundleWriter(string rootPath) : ILocBundleConsumer<FlatEntry>
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void Consume(LocBundle<FlatEntry> table)
    {
        foreach (var (language, group) in table)
        {
            var langDir = Path.Combine(rootPath, language);
            Directory.CreateDirectory(langDir);

            foreach (var (tableName, locTable) in group)
            {
                var dict = new Dictionary<string, string>();
                foreach (var entry in locTable)
                {
                    dict[entry.Key] = entry.Value;
                }

                var text = JsonSerializer.Serialize(dict, _options);
                File.WriteAllText(Path.Combine(langDir, tableName + ".json"), text, Encoding.UTF8);
            }
        }
    }
}