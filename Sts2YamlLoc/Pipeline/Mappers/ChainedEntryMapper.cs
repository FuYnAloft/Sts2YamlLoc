using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

/// <summary>
/// Processor that applies all matching converters sequentially for each entry.
/// Requires input and output entry type to be the same so conversions can be chained.
/// </summary>
public sealed class ChainedEntryMapper<TEntry>(params ChainedEntryMapper<TEntry>.Rule[] rules)
    : ILocBundleProcessor<TEntry, TEntry>
    where TEntry : ILocEntry
{
    // Positional record representing a rule (predicate + converters)
    public sealed record Rule(
        Func<string, string, TEntry, bool> Predicate,
        params IReadOnlyCollection<ILocEntryConverter<TEntry, TEntry>> Converters)
    {
        // Convenience ctor: create Rule from languages + tableNames + converters
        public Rule(IReadOnlyCollection<string>? languages, IReadOnlyCollection<string>? tableNames,
            params IReadOnlyCollection<ILocEntryConverter<TEntry, TEntry>> converters)
            : this(CreatePredicate(languages, tableNames), converters)
        {
        }

        // Convenience ctor: create Rule from tableNames + converters
        public Rule(IReadOnlyCollection<string>? tableNames,
            params IReadOnlyCollection<ILocEntryConverter<TEntry, TEntry>> converters)
            : this(null, tableNames, converters)
        {
        }

        private static Func<string, string, TEntry, bool> CreatePredicate(IReadOnlyCollection<string>? languages,
            IReadOnlyCollection<string>? tableNames)
        {
            return (currentLanguage, currentTableName, _) =>
                (languages == null || languages.Contains(currentLanguage)) &&
                (tableNames == null || tableNames.Contains(currentTableName));
        }
    }

    public LocBundle<TEntry> Process(LocBundle<TEntry> table)
    {
        var groups = new Dictionary<string, LocTableGroup<TEntry>>(table.Count);

        foreach (var (language, group) in table)
        {
            var tables = new Dictionary<string, LocTable<TEntry>>(group.Count);

            foreach (var (tableName, locTable) in group)
            {
                var convertedEntries = new List<TEntry>();

                foreach (var entry in locTable)
                {
                    var current = entry;

                    foreach (var (predicate, converters) in rules)
                    {
                        if (predicate(language, tableName, current))
                        {
                            foreach (var conv in converters)
                            {
                                current = conv.Convert(current);
                            }
                        }
                    }

                    convertedEntries.Add(current);
                }

                tables[tableName] = new LocTable<TEntry>(convertedEntries);
            }

            groups[language] = new LocTableGroup<TEntry>(tables);
        }

        return new LocBundle<TEntry>(groups);
    }
}
