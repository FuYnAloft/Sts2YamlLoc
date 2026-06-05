using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Mappers;

namespace Sts2YamlLoc.Reshapers;

public sealed class MinionLibComponentMerger() : LocTableGroupMapper<FlatEntry, FlatEntry>((_, group) =>
    {
        var newDict = new Dictionary<string, LocTable<FlatEntry>>(group);

        if (group.TryGetValue("components", out var componentsTable))
        {
            newDict.Remove("components");
            if (group.TryGetValue("cards", out var cardsTable))
                newDict["cards"] = new LocTable<FlatEntry>([..cardsTable, ..componentsTable]);
            else
                newDict["cards"] = componentsTable;
        }

        return new LocTableGroup<FlatEntry>(newDict);
    }
);
