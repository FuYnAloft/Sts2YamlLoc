using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Mappers;

namespace Sts2YamlLoc.Reshapers;

public sealed class MinionLibComponentSplitter() : LocTableGroupMapper<FlatEntry, FlatEntry>((_, group) =>
    {
        var newDict = new Dictionary<string, LocTable<FlatEntry>>(group);

        if (group.TryGetValue("cards", out var cardsTable))
        {
            var newCardsTable = new LocTable<FlatEntry>(cardsTable.Where(e => !e.Key.Split('.').FirstOrDefault("").Any(char.IsLower)));
            var newComponentsTable = new LocTable<FlatEntry>(cardsTable.Where(e => e.Key.Split('.').FirstOrDefault("").Any(char.IsLower)));
            newDict["cards"] = newCardsTable;
            newDict["components"] = newComponentsTable;
        }

        return new LocTableGroup<FlatEntry>(newDict);
    }
);
