using ProgrammingLearningApp.Cli;
using ProgrammingLearningApp.Examples;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Metrics;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ConsoleMenuTests
{
    private static string Run(string input)
    {
        var output = new StringWriter();
        var menu = new ConsoleMenu(
            new StringReader(input),
            output,
            new ExampleProgramFactory(new Random(1)),
            new ProgramParser(),
            new ProgramExecutor(),
            new MetricsCalculator());

        menu.Run();
        return output.ToString();
    }

    private static string WriteTempFile(string contents)
    {
        var path = Path.Combine(Path.GetTempPath(), $"menu-test-{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, contents);
        return path;
    }

    private static int CountOccurrences(string text, string value) =>
        (text.Length - text.Replace(value, string.Empty).Length) / value.Length;

    [Fact]
    public void Quit_EndsTheProgram()
    {
        Assert.Contains("Goodbye!", Run("0\n"));
    }

    [Fact]
    public void EndOfInput_EndsTheProgramWithoutCrashing()
    {
        Assert.Contains("Goodbye!", Run(""));
    }

    [Fact]
    public void InvalidChoice_ShowsMessageAndAsksAgain()
    {
        var output = Run("9\n0\n");

        Assert.Contains("Invalid choice", output);
        Assert.Equal(2, CountOccurrences(output, "Step 1:"));
    }

    [Theory]
    [InlineData("1", "basic")]
    [InlineData("2", "advanced")]
    [InlineData("3", "expert")]
    public void RandomExample_IsLoadedAndCanBeExecuted(string choice, string level)
    {
        var output = Run($"{choice}\n1\n0\n0\n");

        Assert.Contains($"Loaded {level} example", output);
        Assert.Contains("End state", output);
    }

    [Fact]
    public void BackInStep2_ReturnsToStep1()
    {
        var output = Run("1\n0\n0\n");

        Assert.Equal(2, CountOccurrences(output, "Step 1:"));
    }

    [Fact]
    public void ImportedFile_CanBeExecuted()
    {
        var path = WriteTempFile(TestPrograms.Example2);
        try
        {
            var output = Run($"4\n{path}\n1\n0\n0\n");

            Assert.Contains("Imported program", output);
            Assert.Contains(
                "Move 10, Turn right, Move 10, Turn right, Move 10, Turn right, Move 10, Turn right.",
                output);
            Assert.Contains("End state (0,0) facing east.", output);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ImportedFile_MetricsAreShown()
    {
        var path = WriteTempFile(TestPrograms.Example3);
        try
        {
            var output = Run($"4\n{path}\n2\n0\n0\n");

            Assert.Contains("No. of commands: 11", output);
            Assert.Contains("Max nesting: 2", output);
            Assert.Contains("No. of repeats: 2", output);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ImportPathInQuotes_IsAccepted()
    {
        var path = WriteTempFile(TestPrograms.Example1);
        try
        {
            Assert.Contains("Imported program", Run($"4\n\"{path}\"\n0\n0\n"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ImportMissingFile_ShowsErrorAndContinues()
    {
        var output = Run("4\n/this/file/does/not/exist.txt\n0\n");

        Assert.Contains("Could not import the file", output);
        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void ImportInvalidProgram_ShowsTheLineWithTheError()
    {
        var path = WriteTempFile("Move 1\nJump 3");
        try
        {
            var output = Run($"4\n{path}\n0\n");

            Assert.Contains("Could not import the file", output);
            Assert.Contains("Line 2", output);
        }
        finally
        {
            File.Delete(path);
        }
    }

    //a RepeatUntil without grid never stops by itself, the step limit stops it and the menu shows the error
    [Fact]
    public void RuntimeError_IsShownAndTheMenuKeepsRunning()
    {
        var path = WriteTempFile("RepeatUntil WallAhead\n    Turn left");
        try
        {
            var output = Run($"4\n{path}\n1\n0\n0\n");

            Assert.Contains("Runtime error:", output);
            Assert.Contains("Goodbye!", output);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
