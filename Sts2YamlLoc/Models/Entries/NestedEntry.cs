namespace Sts2YamlLoc.Models.Entries;

public record NestedEntry(IReadOnlyList<string> Key, string Value) : ILocEntry
{
    object ILocEntry.Key => Key;
}
