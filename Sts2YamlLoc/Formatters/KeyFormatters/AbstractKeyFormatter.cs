using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Formatters.KeyFormatters;

public abstract class AbstractKeyFormatter :
    ILocEntryConverter<FlatEntry, NestedEntry>,
    ILocEntryConverter<NestedEntry, FlatEntry>
{
    public NestedEntry Convert(FlatEntry entry)
    {
        var result = FlatToNested(entry);
        var revert = NestedToFlat(result);
        return revert == entry
            ? result
            : throw new InvalidOperationException($"Conversion is not reversible: {entry} -> {result} -> {revert}");
    }

    public FlatEntry Convert(NestedEntry entry)
    {
        var result = NestedToFlat(entry);
        var revert = FlatToNested(result);
        return revert == entry
            ? result
            : throw new InvalidOperationException($"Conversion is not reversible: {entry} -> {result} -> {revert}");
    }

    protected abstract NestedEntry FlatToNested(FlatEntry entry);
    protected abstract FlatEntry NestedToFlat(NestedEntry entry);
}
