using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

public sealed class LocTableMapper<TEntryIn, TEntryOut>(
    Func<string, string, LocTable<TEntryIn>, LocTable<TEntryOut>> mapper)
    : ILocBundleProcessor<TEntryIn, TEntryOut>
    where TEntryIn : ILocEntry
    where TEntryOut : ILocEntry
{
    public LocBundle<TEntryOut> Process(LocBundle<TEntryIn> table)
    {
        var groups = new Dictionary<string, LocTableGroup<TEntryOut>>(table.Count);

        foreach (var (language, group) in table)
        {
            var tables = new Dictionary<string, LocTable<TEntryOut>>(group.Count);

            foreach (var (tableName, locTable) in group)
            {
                var mappedTable = mapper(language, tableName, locTable);
                tables[tableName] = mappedTable;
            }

            groups[language] = new LocTableGroup<TEntryOut>(tables);
        }

        return new LocBundle<TEntryOut>(groups);
    }
}
