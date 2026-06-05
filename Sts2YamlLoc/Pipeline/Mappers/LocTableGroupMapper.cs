using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

public class LocTableGroupMapper<TEntryIn, TEntryOut>(
    Func<string, LocTableGroup<TEntryIn>, LocTableGroup<TEntryOut>> mapper)
    : ILocBundleProcessor<TEntryIn, TEntryOut>
    where TEntryIn : ILocEntry
    where TEntryOut : ILocEntry
{
    public LocBundle<TEntryOut> Process(LocBundle<TEntryIn> table)
    {
        var groups = new Dictionary<string, LocTableGroup<TEntryOut>>(table.Count);

        foreach (var (language, group) in table)
        {
            var mappedGroup = mapper(language, group);
            groups[language] = mappedGroup;
        }

        return new LocBundle<TEntryOut>(groups);
    }
}
