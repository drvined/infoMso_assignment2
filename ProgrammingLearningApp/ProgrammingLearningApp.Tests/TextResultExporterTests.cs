using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Export;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class TextResultExporterTests
{
    [Fact]
    public void ToText_ContainsTraceAndEndState()
    {
        var program = new ProgramParser().Parse("test", "Move 2");
        var result = new ProgramExecutor().Execute(program);

        var text = new TextResultExporter().ToText(result);

        Assert.Contains("Move 2.", text);
        Assert.Contains("End state (2,0) facing east.", text);
    }

    [Fact]
    public void Save_WritesTheTextToFile()
    {
        var program = new ProgramParser().Parse("test", "Move 2");
        var result = new ProgramExecutor().Execute(program);
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        try
        {
            new TextResultExporter().Save(result, path);

            Assert.Equal(new TextResultExporter().ToText(result), File.ReadAllText(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}