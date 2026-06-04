using System.Collections;
using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Models.Loc;

public class LocTable<TEntry>(IEnumerable<TEntry> entries) : IEnumerable<TEntry>
    where TEntry : ILocEntry
{
    public IEnumerator<TEntry> GetEnumerator() => entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
