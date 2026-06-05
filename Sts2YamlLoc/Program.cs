using Cocona;
using Sts2YamlLoc.Formatters;
using Sts2YamlLoc.Formatters.ModelId;
using Sts2YamlLoc.Models.Entries;
using Sts2YamlLoc.Models.Loc;
using Sts2YamlLoc.Options;
using Sts2YamlLoc.Pipeline.Mappers;
using Sts2YamlLoc.Reshapers;

var app = CoconaApp.Create();


app.AddCommand("baselib", (
    [Argument(Description = "输入的本地化根目录")] string inputDir,
    [Argument(Description = "输出的本地化根目录")] string outputDir,
    [Argument(Description = "命名空间的第一段")] string namespaceTop,
    [Option('d', Description = "转换方向")] ConversionDirection direction = ConversionDirection.YamlToJson,
    [Option('r', Description = "若启用，反转输入输出路径和转换方向")]
    bool reverse = false) =>
{
    if (reverse)
    {
        (inputDir, outputDir) = (outputDir, inputDir);
        direction = direction.Reversed;
    }

    if (direction.IsNestedToFlat)
    {
        var bundleFormatters = new EntryMapper<NestedEntry, FlatEntry>(
            new(
            [
                "cards", "card_keywords", "enchantments", "events", "powers",
                "relics", "static_hover_tips", "characters", "monsters"
            ], new BaseLibIdFormatter(namespaceTop)),
            new(["gameplay_ui", "ui"], new DotFormatter()),
            new(["ancients"], new BaseLibIdFormatter(namespaceTop, pos: 2)),
            new(["components"], new MinionLibComponentIdFormatter(namespaceTop))
        );

        LocBundle
            .Create(direction.GetNestedReader(inputDir))
            .Pipe(bundleFormatters)
            .Pipe(new MinionLibComponentMerger())
            .Sink(direction.GetFlatWriter(outputDir));
        return 0;
    }

    if (direction.IsFlatToNested)
    {
        var bundleFormatters = new EntryMapper<FlatEntry, NestedEntry>(
            new(
            [
                "cards", "card_keywords", "enchantments", "events", "powers",
                "relics", "static_hover_tips", "characters", "monsters"
            ], new BaseLibIdFormatter(namespaceTop)),
            new(["gameplay_ui", "ui"], new DotFormatter()),
            new(["ancients"], new BaseLibIdFormatter(namespaceTop, pos: 2)),
            new(["components"], new MinionLibComponentIdFormatter(namespaceTop))
        );

        LocBundle
            .Create(direction.GetFlatReader(inputDir))
            .Pipe(new MinionLibComponentSplitter())
            .Pipe(bundleFormatters)
            .Sink(direction.GetNestedWriter(outputDir));
        return 0;
    }

    if (direction.IsNestedToNested)
    {
        LocBundle
            .Create(direction.GetNestedReader(inputDir))
            .Sink(direction.GetNestedWriter(outputDir));
        return 0;
    }

    Console.Error.WriteLine("未知的转换方向。");
    return 1;
});

app.Run();
