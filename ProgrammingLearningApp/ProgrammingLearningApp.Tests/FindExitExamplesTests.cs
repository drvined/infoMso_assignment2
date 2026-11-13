using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Examples;
using ProgrammingLearningApp.Metrics;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class FindExitExamplesTests
{
    private static GridProgram Find(string name) =>
        new ExampleProgramFactory().GetAll().Single(p => p.Name == name);

    [Theory]
    [InlineData("FindExit1", 4, 2, 2)]
    [InlineData("FindExit2", 4, 2, 2)]
    [InlineData("FindExit3", 3, 1, 1)]
    public void FindExitExample_HasExpectedMetrics(string name, int commands, int maxNesting, int repeats)
    {
        var metrics = new MetricsCalculator().Calculate(Find(name));

        Assert.Equal(new ProgramMetrics(commands, maxNesting, repeats), metrics);
    }
}
