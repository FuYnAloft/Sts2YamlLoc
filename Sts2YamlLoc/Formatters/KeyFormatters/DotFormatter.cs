using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Formatters.KeyFormatters;

public class DotFormatter : AbstractKeyFormatter
{
    public static DotFormatter Instance { get; } = new();

    protected override NestedEntry FlatToNested(FlatEntry entry)
    {
        return new NestedEntry(entry.Key.Split('.'), entry.Value);
    }

    protected override FlatEntry NestedToFlat(NestedEntry entry)
    {
        return new FlatEntry(string.Join('.', entry.Key), entry.Value);
    }
}
