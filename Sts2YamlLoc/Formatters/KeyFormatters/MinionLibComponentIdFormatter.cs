using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Formatters.KeyFormatters;

public sealed class MinionLibComponentIdFormatter(string namespaceTop) : AbstractKeyFormatter
{
    protected override NestedEntry FlatToNested(FlatEntry entry)
    {
        var parts = entry.Key.Split('.');
        if (parts.FirstOrDefault() != namespaceTop)
            throw new ArgumentException(
                $"Key '{entry.Key}' does not start with the expected namespace '{namespaceTop}'.");
        if (parts.Length < 2)
            throw new ArgumentException(
                $"Key '{entry.Key}' does not have enough segments to extract the model ID after the namespace '{namespaceTop}'.");
        return new NestedEntry(parts[1..], entry.Value);
    }

    protected override FlatEntry NestedToFlat(NestedEntry entry)
    {
        return new FlatEntry(string.Join('.', [namespaceTop, ..entry.Key]), entry.Value);
    }
}
