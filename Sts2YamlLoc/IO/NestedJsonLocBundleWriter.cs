using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.IO;

public sealed class NestedJsonLocBundleWriter(string rootPath) : ILocBundleConsumer<NestedEntry>
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void Consume(LocBundle<NestedEntry> table)
    {
        foreach (var (language, group) in table)
        {
            var langDir = Path.Combine(rootPath, language);
            Directory.CreateDirectory(langDir);

            foreach (var (tableName, locTable) in group)
            {
                var mapping = StructureHelper.BuildNestedMapping(locTable);
                var text = JsonSerializer.Serialize(mapping, _options);
                File.WriteAllText(Path.Combine(langDir, tableName + ".json"), text, Encoding.UTF8);
            }
        }
    }
}
