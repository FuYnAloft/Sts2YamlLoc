using Sts2YamlLoc.Models.Entries;

namespace Sts2YamlLoc.Pipeline.Interfaces;

public interface ILocEntryConverter<in TIn, out TOut>
    where TIn : ILocEntry
    where TOut : ILocEntry
{
    TOut Convert(TIn entry);
}
