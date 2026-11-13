using ProgrammingLearningApp.Export;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class TextProgramExporterTests
{
    private const string Source = "Repeat 2 times\n    Move 3\n    Turn right\nRepeatUntil WallAhead\n    Move 1\n    Turn left";

    [Fact]
    public void ToText_WritesTheSameProgramAsTheSource()
    {
        var program = new ProgramParser().Parse("test", Source);

        var text = new TextProgramExporter().ToText(program);

        Assert.Equal(Source.Replace("\n", Environment.NewLine) + Environment.NewLine, text);
    }

    [Fact]
    public void ToText_OutputCanBeParsedAgainToTheSameText()
    {
        var program = new ProgramParser().Parse("test", Source);
        var text = new TextProgramExporter().ToText(program);

        var parsedAgain = new ProgramParser().Parse("test", text);

        Assert.Equal(text, new TextProgramExporter().ToText(parsedAgain));
    }
}
