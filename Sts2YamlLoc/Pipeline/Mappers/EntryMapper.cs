using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

/// <summary>
/// Processor that applies all matching converters sequentially for each entry.
/// Requires input and output entry type to be the same so conversions can be chained.
/// </summary>
public sealed class EntryMapper<TEntry>(
    params (Func<string, string, TEntry, bool> Predicate, IEnumerable<ILocEntryConverter<TEntry, TEntry>> Converters)[] rules)
    : ILocBundleProcessor<TEntry, TEntry>
    where TEntry : ILocEntry
{
    // Nested record types used by the convenience constructors
    public sealed record Rule(IReadOnlyCollection<string>? Languages, IReadOnlyCollection<string>? TableNames, IEnumerable<ILocEntryConverter<TEntry, TEntry>> Converters);
    public sealed record TableRule(IReadOnlyCollection<string>? TableNames, IEnumerable<ILocEntryConverter<TEntry, TEntry>> Converters);

    public EntryMapper(params Rule[] rules)
        : this(rules.Select(r => (CreatePredicate(r.Languages, r.TableNames), r.Converters)).ToArray())
    {
    }

    public EntryMapper(params TableRule[] rules)
        : this(rules.Select(r => (CreatePredicateForTables(r.TableNames), r.Converters)).ToArray())
    {
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

    private static Func<string, string, TEntry, bool> CreatePredicate(IReadOnlyCollection<string>? languages, IReadOnlyCollection<string>? tableNames)
    {
        return (currentLanguage, currentTableName, _) =>
            (languages == null || languages.Contains(currentLanguage)) && (tableNames == null || tableNames.Contains(currentTableName));
    }

    private static Func<string, string, TEntry, bool> CreatePredicateForTables(IReadOnlyCollection<string>? tableNames)
    {
        return (_, currentTableName, _) => tableNames == null || tableNames.Contains(currentTableName);
    }
}
