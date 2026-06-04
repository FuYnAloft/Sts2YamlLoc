using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;

namespace Sts2YamlLoc.Pipeline.Interfaces;

public interface ILocBundleProducer<TEntry> where TEntry : ILocEntry
{
    LocBundle<TEntry> Produce();

    public class FromDelegate(Func<LocBundle<TEntry>> func) : ILocBundleProducer<TEntry>
    {
        public LocBundle<TEntry> Produce() => func();
    }
}
