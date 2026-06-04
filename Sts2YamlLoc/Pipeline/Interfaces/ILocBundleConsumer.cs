using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;

namespace Sts2YamlLoc.Pipeline.Interfaces;

public interface ILocBundleConsumer<TEntry> where TEntry : ILocEntry
{
    void Consume(LocBundle<TEntry> table);

    public class FromDelegate(Action<LocBundle<TEntry>> action) : ILocBundleConsumer<TEntry>
    {
        public void Consume(LocBundle<TEntry> table) => action(table);
    }
}
