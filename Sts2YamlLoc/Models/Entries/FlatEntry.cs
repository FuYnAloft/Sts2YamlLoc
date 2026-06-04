namespace Sts2YamlLoc.Models.Entries;

public record FlatEntry(string Key, string Value) : ILocEntry
{
    object ILocEntry.Key => Key;
}
