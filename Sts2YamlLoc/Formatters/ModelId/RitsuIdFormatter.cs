using System.Text.RegularExpressions;

namespace Sts2YamlLoc.Formatters.ModelId;

public sealed partial class RitsuIdFormatter(string modid, string category, int pos = 0)
    : PrefixedIdFormatter(BuildPrefix(modid, category), pos)
{
    public static string BuildPrefix(string modid, string category) =>
        $"{NormalizePublicStem(modid)}_{SlugifyCategory(category)}_";

    [GeneratedRegex("[^A-Za-z0-9]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex("([A-Z]+)([A-Z][a-z])")]
    private static partial Regex AcronymBoundaryRegex();

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex CamelBoundaryRegex();

    [GeneratedRegex("_+")]
    private static partial Regex RepeatedUnderscoreRegex();

    public static string NormalizePublicStem(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = NonAlphaNumericRegex().Replace(value.Trim(), "_");
        normalized = AcronymBoundaryRegex().Replace(normalized, "$1_$2");
        normalized = CamelBoundaryRegex().Replace(normalized, "$1_$2");
        normalized = RepeatedUnderscoreRegex().Replace(normalized, "_");
        return normalized.Trim('_').ToUpperInvariant();
    }

    public static string SlugifyCategory(string category)
    {
        if (category.All(char.IsUpper)) return category;

        var slug = VanillaIdFormatter.Slugify(category);
        return slug.EndsWith("_MODEL") ? slug[..^"_MODEL".Length] : slug;
    }
}
