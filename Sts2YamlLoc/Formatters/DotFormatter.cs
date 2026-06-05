using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Formatters;

public class DotFormatter : AbstractFormatter
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
