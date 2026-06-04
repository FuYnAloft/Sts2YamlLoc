using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Pipeline.Interfaces;

public interface ILocEntryConverter<TIn, TOut>
    where TIn : ILocEntry
    where TOut : ILocEntry
{
    TOut Convert(TIn entry);

    public class FromDelegate(Func<TIn, TOut> func) : ILocEntryConverter<TIn, TOut>
    {
        public TOut Convert(TIn entry) => func(entry);
    }
}
