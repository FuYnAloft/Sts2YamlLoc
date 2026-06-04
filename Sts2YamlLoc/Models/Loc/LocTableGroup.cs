using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Models.Loc;

public class LocTableGroup<TEntry>(IReadOnlyDictionary<string, LocTable<TEntry>> tables)
    : IReadOnlyDictionary<string, LocTable<TEntry>>
    where TEntry : ILocEntry
{
    public IEnumerator<KeyValuePair<string, LocTable<TEntry>>> GetEnumerator() => tables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => tables.Count;
    public bool ContainsKey(string tableName) => tables.ContainsKey(tableName);

    public bool TryGetValue(string tableName, [MaybeNullWhen(false)] out LocTable<TEntry> value) =>
        tables.TryGetValue(tableName, out value);

    public LocTable<TEntry> this[string tableName] => tables[tableName];
    public IEnumerable<string> Keys => tables.Keys;
    public IEnumerable<LocTable<TEntry>> Values => tables.Values;
}
