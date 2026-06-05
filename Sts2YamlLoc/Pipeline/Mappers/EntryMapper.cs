using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Pipeline.Mappers;

/// <summary>
/// Processor that applies the first matching converter for each entry.
/// Supports different input and output entry types (TEntryIn -> TEntryOut).
/// </summary>
public sealed class EntryMapper<TEntryIn, TEntryOut>(params EntryMapper<TEntryIn, TEntryOut>.Rule[] rules)
    : ILocBundleProcessor<TEntryIn, TEntryOut>
    where TEntryIn : ILocEntry
    where TEntryOut : ILocEntry
{
    // Positional record representing a rule (predicate + converter)
    public sealed record Rule(
        Func<string, string, TEntryIn, bool> Predicate,
        ILocEntryConverter<TEntryIn, TEntryOut> Converter)
    {
        // Convenience ctor: create Rule from languages + tableNames + converter
        public Rule(IReadOnlyCollection<string>? languages, IReadOnlyCollection<string>? tableNames,
            ILocEntryConverter<TEntryIn, TEntryOut> converter)
            : this(CreatePredicate(languages, tableNames), converter)
        {
        }

        // Convenience ctor: create Rule from tableNames + converter
        public Rule(IReadOnlyCollection<string>? tableNames, ILocEntryConverter<TEntryIn, TEntryOut> converter)
            : this(null, tableNames, converter)
        {
        }

        private static Func<string, string, TEntryIn, bool> CreatePredicate(IReadOnlyCollection<string>? languages,
            IReadOnlyCollection<string>? tableNames)
        {
            return (currentLanguage, currentTableName, _) =>
                (languages == null || languages.Contains(currentLanguage)) &&
                (tableNames == null || tableNames.Contains(currentTableName));
        }
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
                        throw new InvalidOperationException(
                            $"Entry {entry} in language '{language}', table '{tableName}' did not match any converter rule.");
                    }
                }

                tables[tableName] = new LocTable<TEntryOut>(convertedEntries);
            }

            groups[language] = new LocTableGroup<TEntryOut>(tables);
        }

        return new LocBundle<TEntryOut>(groups);
    }
}
