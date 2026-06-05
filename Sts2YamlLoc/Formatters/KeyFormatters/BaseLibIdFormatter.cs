namespace Sts2YamlLoc.Formatters.KeyFormatters;

public sealed class BaseLibIdFormatter(string namespaceTop, int pos = 0)
    : PrefixedIdFormatter($"{namespaceTop.ToUpperInvariant()}-", pos);
