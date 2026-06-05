using Sts2YamlLoc.IO;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Pipeline.Interfaces;

namespace Sts2YamlLoc.Options;

public enum ConversionDirection
{
    YamlToJson,
    JsonToYaml,
    NestedJsonToJson,
    JsonToNestedJson,
    YamlToNestedJson,
    NestedJsonToYaml
}

public static class ConversionDirectionExtensions
{
    extension(ConversionDirection direction)
    {
        public bool IsNestedToFlat => direction switch
        {
            ConversionDirection.YamlToJson => true,
            ConversionDirection.NestedJsonToJson => true,
            _ => false
        };

        public bool IsFlatToNested => direction switch
        {
            ConversionDirection.JsonToYaml => true,
            ConversionDirection.JsonToNestedJson => true,
            _ => false
        };

        public bool IsNestedToNested => direction switch
        {
            ConversionDirection.YamlToNestedJson => true,
            ConversionDirection.NestedJsonToYaml => true,
            _ => false
        };

        public ConversionDirection Reversed =>
            direction switch
            {
                ConversionDirection.YamlToJson => ConversionDirection.JsonToYaml,
                ConversionDirection.JsonToYaml => ConversionDirection.YamlToJson,

                ConversionDirection.NestedJsonToJson => ConversionDirection.JsonToNestedJson,
                ConversionDirection.JsonToNestedJson => ConversionDirection.NestedJsonToJson,

                ConversionDirection.YamlToNestedJson => ConversionDirection.NestedJsonToYaml,
                ConversionDirection.NestedJsonToYaml => ConversionDirection.YamlToNestedJson,

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        public ILocBundleProducer<FlatEntry> GetFlatReader(string rootPath)
        {
            return direction switch
            {
                ConversionDirection.JsonToYaml => new JsonLocBundleReader(rootPath),
                ConversionDirection.JsonToNestedJson => new JsonLocBundleReader(rootPath),
                _ => throw new InvalidOperationException($"Cannot read flat entries from {direction}")
            };
        }

        public ILocBundleProducer<NestedEntry> GetNestedReader(string rootPath)
        {
            return direction switch
            {
                ConversionDirection.YamlToJson => new YamlLocBundleReader(rootPath),
                ConversionDirection.YamlToNestedJson => new YamlLocBundleReader(rootPath),
                ConversionDirection.NestedJsonToJson => new NestedJsonLocBundleReader(rootPath),
                ConversionDirection.NestedJsonToYaml => new NestedJsonLocBundleReader(rootPath),
                _ => throw new InvalidOperationException($"Cannot read nested entries from {direction}")
            };
        }

        public ILocBundleConsumer<FlatEntry> GetFlatWriter(string rootPath)
        {
            return direction switch
            {
                ConversionDirection.YamlToJson => new JsonLocBundleWriter(rootPath),
                ConversionDirection.NestedJsonToJson => new JsonLocBundleWriter(rootPath),
                _ => throw new InvalidOperationException($"Cannot write flat entries to {direction}")
            };
        }

        public ILocBundleConsumer<NestedEntry> GetNestedWriter(string rootPath)
        {
            return direction switch
            {
                ConversionDirection.JsonToYaml => new YamlLocBundleWriter(rootPath),
                ConversionDirection.JsonToNestedJson => new NestedJsonLocBundleWriter(rootPath),
                ConversionDirection.YamlToNestedJson => new NestedJsonLocBundleWriter(rootPath),
                ConversionDirection.NestedJsonToYaml => new YamlLocBundleWriter(rootPath),
                _ => throw new InvalidOperationException($"Cannot write nested entries to {direction}")
            };
        }
    }
}
