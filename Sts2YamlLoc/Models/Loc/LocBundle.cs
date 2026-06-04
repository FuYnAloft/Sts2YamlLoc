using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Models.Loc;

public class LocBundle<TEntry>(IReadOnlyDictionary<string, LocTableGroup<TEntry>> groups)
    : IReadOnlyDictionary<string, LocTableGroup<TEntry>>
    where TEntry : ILocEntry
{
    public static LocBundle<T> Create<T>(ILocBundleProducer<T> producer) where T : ILocEntry
    {
        return producer.Produce();
    }

    public LocBundle<TEntryOut> Pipe<TEntryOut>(ILocBundleProcessor<TEntry, TEntryOut> processor)
        where TEntryOut : ILocEntry
    {
        return processor.Process(this);
    }

    public LocBundle<TEntry> Sink(ILocBundleConsumer<TEntry> consumer)
    {
        consumer.Consume(this);
        return this;
    }

    // 实现 IReadOnlyDictionary 接口
    public IEnumerator<KeyValuePair<string, LocTableGroup<TEntry>>> GetEnumerator() =>
        groups.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => groups.Count;
    public bool ContainsKey(string language) => groups.ContainsKey(language);

    public bool TryGetValue(string language,
        [MaybeNullWhen(false)] out LocTableGroup<TEntry> value) =>
        groups.TryGetValue(language, out value);

    public LocTableGroup<TEntry> this[string language] => groups[language];
    public IEnumerable<string> Keys => groups.Keys;
    public IEnumerable<LocTableGroup<TEntry>> Values => groups.Values;
}
