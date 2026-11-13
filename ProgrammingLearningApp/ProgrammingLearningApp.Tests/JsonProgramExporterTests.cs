using System.Text.Json;
using ProgrammingLearningApp.Export;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class JsonProgramExporterTests
{
    [Fact]
    public void Export_WritesNameAndNestedCommands()
    {
        var program = new ProgramParser().Parse("test", "Repeat 2 times\n    Move 3\n    Turn right");

        using var document = JsonDocument.Parse(new JsonProgramExporter().Export(program));
        var root = document.RootElement;

        Assert.Equal("test", root.GetProperty("name").GetString());

        var repeat = root.GetProperty("commands")[0];
        Assert.Equal("Repeat", repeat.GetProperty("type").GetString());
        Assert.Equal(2, repeat.GetProperty("times").GetInt32());

        var move = repeat.GetProperty("commands")[0];
        Assert.Equal("Move", move.GetProperty("type").GetString());
        Assert.Equal(3, move.GetProperty("steps").GetInt32());
    }

    [Fact]
    public void Export_WritesRepeatUntilWithItsCondition()
    {
        var program = new ProgramParser().Parse("test", "Repeat until GridEdge\n    Move 1");

        using var document = JsonDocument.Parse(new JsonProgramExporter().Export(program));
        var loop = document.RootElement.GetProperty("commands")[0];

        Assert.Equal("RepeatUntil", loop.GetProperty("type").GetString());
        Assert.Equal("GridEdge", loop.GetProperty("condition").GetString());
    }
}
