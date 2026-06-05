using Cocona;
using Sts2YamlLoc.Formatters;
using Sts2YamlLoc.Formatters.ModelId;
using Sts2YamlLoc.IO;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Pipeline.Mappers;

var app = CoconaApp.Create();


app.AddCommand("baselib", (
    [Argument(Description = "输入的本地化根目录")] string inputDir,
    [Argument(Description = "输出的本地化根目录")] string outputDir,
    [Argument(Description = "命名空间的第一段")] string namespaceTop,
    [Option(Description = "转换方向")] ConversionDirection direction = ConversionDirection.YamlToJson) =>
{
    switch (direction)
    {
        case ConversionDirection.YamlToJson:
        {
            var reader = new YamlLocBundleReader(inputDir);
            var writer = new JsonLocBundleWriter(outputDir);
            var bundleFormatters = new EntryMapper<NestedEntry, FlatEntry>(
                new(
                [
                    "cards", "card_keywords", "enchantments", "events", "powers",
                    "relics", "static_hover_tips", "characters", "monsters"
                ], new BaseLibIdFormatter(namespaceTop)),
                new(["gameplay_ui", "ui"], new DotFormatter()),
                new(["ancients"], new BaseLibIdFormatter(namespaceTop, pos: 2))
            );

            LocBundle<NestedEntry>
                .Create(reader)
                .Pipe(bundleFormatters)
                .Sink(writer);
            return 0;
        }
        case ConversionDirection.JsonToYaml:
        {
            var reader = new JsonLocBundleReader(inputDir);
            var writer = new YamlLocBundleWriter(outputDir);
            var bundleFormatters = new EntryMapper<FlatEntry, NestedEntry>(
                new(
                [
                    "cards", "card_keywords", "enchantments", "events", "powers",
                    "relics", "static_hover_tips", "characters", "monsters"
                ], new BaseLibIdFormatter(namespaceTop)),
                new(["gameplay_ui", "ui"], new DotFormatter()),
                new(["ancients"], new BaseLibIdFormatter(namespaceTop, pos: 2))
            );

            LocBundle<FlatEntry>
                .Create(reader)
                .Pipe(bundleFormatters)
                .Sink(writer);
            return 0;
        }
        default:
            Console.Error.WriteLine("未知的转换方向。");
            return 1;
    }
});

app.Run();

internal enum ConversionDirection
{
    YamlToJson,
    JsonToYaml
}
