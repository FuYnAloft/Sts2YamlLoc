using System.Text;

namespace Sts2YamlLoc.Models.Entries;

public record NestedEntry(IReadOnlyList<string> Key, string Value) : ILocEntry
{
    object ILocEntry.Key => Key;

    public virtual bool Equals(NestedEntry? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Value == other.Value && SequenceEqual(Key, other.Key);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Value);

        foreach (var item in Key)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    private static bool SequenceEqual(IReadOnlyList<string>? x, IReadOnlyList<string>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        if (x.Count != y.Count) return false;

        return !x.Where((t, i) => t != y[i]).Any();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        var keyStr = $"[{string.Join(", ", Key)}]";

        builder.Append($"Key = {keyStr}, ");
        builder.Append($"Value = {Value}");
        return true;
    }
}
