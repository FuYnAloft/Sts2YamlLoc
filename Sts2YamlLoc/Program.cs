using Cocona;

var app = CoconaApp.Create();

// Cocona 示例代码
app.AddCommand("sync", (string path) =>
{
    Console.WriteLine($"正在同步路径: {path}");
});

app.AddCommand("export", (string format = "yaml") =>
{
    Console.WriteLine($"导出格式: {format}");
});

app.Run();
