using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

public sealed class IdentityMapper<TEntry> : ILocBundleProcessor<TEntry, TEntry>
    where TEntry : ILocEntry
{
    public static IdentityMapper<TEntry> Instance { get; } = new();

    public LocBundle<TEntry> Process(LocBundle<TEntry> table) => table;
}
