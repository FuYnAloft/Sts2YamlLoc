using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.IO;

public static class StructureHelper
{
    public static IList<NestedEntry> ExtractEntries(object? node)
    {
        var result = new List<NestedEntry>();
        if (node is not IDictionary<object, object> dict)
            return result;

        Recurse(dict, []);
        return result;

        void Recurse(IDictionary<object, object> current, List<string> path)
        {
            foreach (var kv in current)
            {
                var key = kv.Key.ToString() ?? string.Empty;
                var value = kv.Value;
                var newPath = new List<string>(path) { key };

                switch (value)
                {
                    case IDictionary<object, object> childDict:
                        Recurse(childDict, newPath);
                        break;
                    case string s:
                        result.Add(new NestedEntry(newPath, s));
                        break;
                    default:
                        // other scalar types
                        result.Add(new NestedEntry(newPath, value.ToString() ?? string.Empty));
                        break;
                }
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
}
