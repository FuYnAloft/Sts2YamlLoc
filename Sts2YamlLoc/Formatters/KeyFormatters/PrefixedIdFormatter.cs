using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Formatters.KeyFormatters;

public class PrefixedIdFormatter(string prefix, int pos = 0) : AbstractKeyFormatter
{
    private readonly int _prefixLength = prefix.Length;

    protected override NestedEntry FlatToNested(FlatEntry entry)
    {
        var key = entry.Key.Split('.');
        if (key.Length <= pos)
            throw new ArgumentException(
                $"Key '{entry.Key}' does not have enough segments to extract the model ID at position {pos}.");
        if (!key[pos].StartsWith(prefix))
            throw new ArgumentException(
                $"Key '{entry.Key}' does not have the expected prefix '{prefix}' at position {pos}.");

        key[pos] = VanillaIdFormatter.Unslugify(key[pos][_prefixLength..]);
        return new NestedEntry(key, entry.Value);
    }

    protected override FlatEntry NestedToFlat(NestedEntry entry)
    {
        var key = entry.Key.ToArray();
        if (key.Length <= pos)
            throw new ArgumentException(
                $"Key '{string.Join('.', entry.Key)}' does not have enough segments to extract the model ID at position {pos}.");
        key[pos] = prefix + VanillaIdFormatter.Slugify(key[pos]);
        return new FlatEntry(string.Join('.', key), entry.Value);
    }
}
