using System.Text.Json;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Export;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class JsonResultExporterTests
{
    [Fact]
    public void ToJson_ContainsTraceEndStateAndPath()
    {
        var program = new ProgramParser().Parse("test", "Move 2");
        var result = new ProgramExecutor().Execute(program);

        using var document = JsonDocument.Parse(new JsonResultExporter().ToJson(result));
        var root = document.RootElement;

        Assert.Equal("Move 2", root.GetProperty("trace")[0].GetString());

        var endState = root.GetProperty("endState");
        Assert.Equal(2, endState.GetProperty("x").GetInt32());
        Assert.Equal(0, endState.GetProperty("y").GetInt32());
        Assert.Equal("east", endState.GetProperty("direction").GetString());

        Assert.Equal(3, root.GetProperty("path").GetArrayLength());
    }

    [Fact]
    public void Save_WritesTheJsonToFile()
    {
        var program = new ProgramParser().Parse("test", "Move 2");
        var result = new ProgramExecutor().Execute(program);
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            new JsonResultExporter().Save(result, path);

            Assert.Equal(new JsonResultExporter().ToJson(result), File.ReadAllText(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}