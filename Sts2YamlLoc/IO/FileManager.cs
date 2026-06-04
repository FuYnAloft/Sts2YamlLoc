using Sts2YamlLoc.Models;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.IO;

public abstract class FileManager<TEntry> : ILocBundleProducer<TEntry>, ILocBundleConsumer<TEntry>
    where TEntry : ILocEntry
{
    public LocBundle<TEntry> Produce() => Load();

    public void Consume(LocBundle<TEntry> table) => Save(table);

    protected abstract LocBundle<TEntry> Load();
    protected abstract void Save(LocBundle<TEntry> table);
}
