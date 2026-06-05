using System.Text.RegularExpressions;
using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Formatters.KeyFormatters;

public sealed partial class VanillaIdFormatter(int pos = 0) : AbstractKeyFormatter
{
    protected override NestedEntry FlatToNested(FlatEntry entry)
    {
        var key = entry.Key.Split('.');
        if (key.Length <= pos)
            throw new ArgumentException(
                $"Key '{entry.Key}' does not have enough segments to extract the model ID at position {pos}.");
        key[pos] = Unslugify(key[pos]);
        return new NestedEntry(key, entry.Value);
    }

    protected override FlatEntry NestedToFlat(NestedEntry entry)
    {
        var key = entry.Key.ToArray();
        if (key.Length <= pos)
            throw new ArgumentException(
                $"Key '{string.Join('.', entry.Key)}' does not have enough segments to extract the model ID at position {pos}.");
        key[pos] = Slugify(key[pos]);
        return new FlatEntry(string.Join('.', key), entry.Value);
    }

    [GeneratedRegex(@"([A-Za-z0-9]|\G(?!^))([A-Z])")]
    private static partial Regex CamelCaseRegex();

    [GeneratedRegex(@"(.*?)_([a-zA-Z0-9])")]
    private static partial Regex SnakeCaseRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"[^A-Z0-9_]")]
    private static partial Regex SpecialCharRegex();

    public static string SnakeCase(string txt)
    {
        return CamelCaseRegex().Replace(txt.Trim(), "$1_$2").ToLowerInvariant();
    }

    public static string Slugify(string txt)
    {
        var str = CamelCaseRegex().Replace(txt.Trim(), "$1_$2");
        var input = WhitespaceRegex().Replace(str.ToUpperInvariant(), "_");
        return SpecialCharRegex().Replace(input, "");
    }

    public static string Unslugify(string txt)
    {
        var str = SnakeCaseRegex().Replace(txt.Trim().ToLowerInvariant(),
            match => match.Groups[1].Value + match.Groups[2].Value.ToUpperInvariant());

        return char.ToUpperInvariant(str[0]) + str[1..];
    }
}
