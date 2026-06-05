using System.Text;
using System.Text.Json;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.IO;

public sealed class NestedJsonLocBundleReader(string rootPath) : ILocBundleProducer<NestedEntry>
{
    public LocBundle<NestedEntry> Produce()
    {
        var groups = new Dictionary<string, LocTableGroup<NestedEntry>>();

        if (!Directory.Exists(rootPath))
            return new LocBundle<NestedEntry>(groups);

        foreach (var langDir in Directory.EnumerateDirectories(rootPath))
        {
            var language = Path.GetFileName(langDir);
            var tables = new Dictionary<string, LocTable<NestedEntry>>();

            foreach (var file in Directory.EnumerateFiles(langDir, "*.json"))
            {
                var tableName = Path.GetFileNameWithoutExtension(file);
                var text = File.ReadAllText(file, Encoding.UTF8);
                IList<NestedEntry> entries = new List<NestedEntry>();

                try
                {
                    using var doc = JsonDocument.Parse(text);
                    var root = doc.RootElement;
                    var obj = ConvertJsonElement(root);
                    if (obj is IDictionary<object, object> dict)
                    {
                        entries = StructureHelper.ExtractEntries(dict);
                    }
                }
                catch
                {
                    // ignore parse errors and treat as empty table
                }

                tables[tableName] = new LocTable<NestedEntry>(entries);
            }

            groups[language] = new LocTableGroup<NestedEntry>(tables);
        }

        return new LocBundle<NestedEntry>(groups);
    }

    private static object? ConvertJsonElement(JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<object, object>();
                foreach (var prop in el.EnumerateObject())
                {
                    dict[prop.Name] = ConvertJsonElement(prop.Value) ?? string.Empty;
                }

                return dict;

            case JsonValueKind.Array:
                // arrays are not typical for localization mapping; preserve as raw JSON text
                return el.GetRawText();

            case JsonValueKind.String:
                return el.GetString();

            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                // represent scalars by their raw JSON text (numbers/booleans/null)
                return el.GetRawText();
        }
    }
}
