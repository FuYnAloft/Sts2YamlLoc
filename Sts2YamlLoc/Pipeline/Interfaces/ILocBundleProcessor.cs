using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;

namespace Sts2YamlLoc.Pipeline.Interfaces;

public interface ILocBundleProcessor<TEntryIn, TEntryOut>
    where TEntryIn : ILocEntry
    where TEntryOut : ILocEntry
{
    LocBundle<TEntryOut> Process(LocBundle<TEntryIn> table);

    public class FromDelegate(Func<LocBundle<TEntryIn>, LocBundle<TEntryOut>> func) : ILocBundleProcessor<TEntryIn, TEntryOut>
    {
        public LocBundle<TEntryOut> Process(LocBundle<TEntryIn> table) => func(table);
    }
}
