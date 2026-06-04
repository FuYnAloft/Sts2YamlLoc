using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline;

/// <summary>
/// Processor that applies the first matching converter for each entry.
/// Supports different input and output entry types (TEntryIn -> TEntryOut).
/// </summary>
public sealed class ChainedEntryMapper<TEntryIn, TEntryOut>(
    params (Func<string, string, TEntryIn, bool> Predicate, ILocEntryConverter<TEntryIn, TEntryOut> Converter)[] rules)
    : ILocBundleProcessor<TEntryIn, TEntryOut>
    where TEntryIn : ILocEntry
    where TEntryOut : ILocEntry
{
    // Nested record types used by the convenience constructors
    public sealed record Rule(IReadOnlyCollection<string>? Languages, IReadOnlyCollection<string>? TableNames, ILocEntryConverter<TEntryIn, TEntryOut> Converter);
    public sealed record TableRule(IReadOnlyCollection<string>? TableNames, ILocEntryConverter<TEntryIn, TEntryOut> Converter);

    public ChainedEntryMapper(params Rule[] rules)
        : this(rules.Select(r => (CreatePredicate(r.Languages, r.TableNames), r.Converter)).ToArray())
    {
    }

    public ChainedEntryMapper(params TableRule[] rules)
        : this(rules.Select(r => (CreatePredicateForTables(r.TableNames), r.Converter)).ToArray())
    {
    }

    public LocBundle<TEntryOut> Process(LocBundle<TEntryIn> table)
    {
        var groups = new Dictionary<string, LocTableGroup<TEntryOut>>(table.Count);

        foreach (var (language, group) in table)
        {
            var tables = new Dictionary<string, LocTable<TEntryOut>>(group.Count);

            foreach (var (tableName, locTable) in group)
            {
                var convertedEntries = new List<TEntryOut>();

                foreach (var entry in locTable)
                {
                    TEntryOut? result = default;
                    var converted = false;

                    foreach (var (predicate, converter) in rules)
                    {
                        if (predicate(language, tableName, entry))
                        {
                            result = converter.Convert(entry);
                            converted = true;
                            break; // stop at first match
                        }
                    }

                    if (converted && result is not null)
                    {
                        convertedEntries.Add(result);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Entry {entry} in language '{language}', table '{tableName}' did not match any converter rule.");
                    }
                }

                tables[tableName] = new LocTable<TEntryOut>(convertedEntries);
            }

            groups[language] = new LocTableGroup<TEntryOut>(tables);
        }

        return new LocBundle<TEntryOut>(groups);
    }

    private static Func<string, string, TEntryIn, bool> CreatePredicate(IReadOnlyCollection<string>? languages, IReadOnlyCollection<string>? tableNames)
    {
        return (currentLanguage, currentTableName, _) =>
            (languages == null || languages.Contains(currentLanguage)) && (tableNames == null || tableNames.Contains(currentTableName));
    }

    private static Func<string, string, TEntryIn, bool> CreatePredicateForTables(IReadOnlyCollection<string>? tableNames)
    {
        return (_, currentTableName, _) => tableNames == null || tableNames.Contains(currentTableName);
    }
}
