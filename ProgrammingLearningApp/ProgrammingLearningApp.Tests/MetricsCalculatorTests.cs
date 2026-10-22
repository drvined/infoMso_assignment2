using ProgrammingLearningApp.Metrics;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class MetricsCalculatorTests
{
    private static ProgramMetrics Measure(string text) =>
        new MetricsCalculator().Calculate(TestPrograms.Parse(text));

    [Fact]
    public void Example1_MatchesTable()
    {
        Assert.Equal(new ProgramMetrics(8, 0, 0), Measure(TestPrograms.Example1));
    }

    [Fact]
    public void Example2_MatchesTable()
    {
        Assert.Equal(new ProgramMetrics(3, 1, 1), Measure(TestPrograms.Example2));
    }

    [Fact]
    public void Example3_MatchesTable()
    {
        Assert.Equal(new ProgramMetrics(11, 2, 2), Measure(TestPrograms.Example3));
    }
}