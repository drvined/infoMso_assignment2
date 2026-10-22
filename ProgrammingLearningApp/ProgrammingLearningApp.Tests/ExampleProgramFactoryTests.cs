using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Examples;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Metrics;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ExampleProgramFactoryTests
{
    private readonly ExampleProgramFactory _factory = new();

    private GridProgram Find(string name) => _factory.GetAll().Single(p => p.Name == name);

    [Theory]
    [InlineData(ExampleLevel.Basic)]
    [InlineData(ExampleLevel.Advanced)]
    [InlineData(ExampleLevel.Expert)]
    public void EveryLevel_HasAtLeastOneProgram(ExampleLevel level)
    {
        Assert.NotEmpty(_factory.Get(level));
    }

    [Fact]
    public void GetAll_ContainsEveryLevel_WithUniqueNames()
    {
        var all = _factory.GetAll();
        var expectedCount = Enum.GetValues<ExampleLevel>().Sum(level => _factory.Get(level).Count);

        Assert.Equal(expectedCount, all.Count);
        Assert.Equal(all.Count, all.Select(p => p.Name).Distinct().Count());
    }

    [Fact]
    public void BasicPrograms_UseNoRepeats()
    {
        foreach (var program in _factory.Get(ExampleLevel.Basic))
        {
            Assert.Equal(0, new MetricsCalculator().Calculate(program).RepeatCount);
        }
    }

    [Fact]
    public void AdvancedPrograms_UseARepeatWithoutNesting()
    {
        foreach (var program in _factory.Get(ExampleLevel.Advanced))
        {
            var metrics = new MetricsCalculator().Calculate(program);
            Assert.True(metrics.RepeatCount > 0, $"{program.Name} has no repeat");
            Assert.Equal(1, metrics.MaxNestingLevel);
        }
    }

    [Fact]
    public void ExpertPrograms_UseNestedRepeats()
    {
        foreach (var program in _factory.Get(ExampleLevel.Expert))
        {
            Assert.True(new MetricsCalculator().Calculate(program).MaxNestingLevel >= 2,
                $"{program.Name} has no nested repeat");
        }
    }

    [Theory]
    [InlineData("Rectangle1", 8, 0, 0)]
    [InlineData("Step", 5, 0, 0)]
    [InlineData("Zigzag", 5, 0, 0)]
    [InlineData("Rectangle2", 3, 1, 1)]
    [InlineData("Staircase", 5, 1, 1)]
    [InlineData("Random", 11, 2, 2)]
    [InlineData("TwoSquares", 5, 2, 2)]
    public void Example_HasExpectedMetrics(string name, int commands, int maxNesting, int repeats)
    {
        var metrics = new MetricsCalculator().Calculate(Find(name));

        Assert.Equal(new ProgramMetrics(commands, maxNesting, repeats), metrics);
    }

    [Theory]
    [InlineData("Rectangle1", 0, 0, Direction.East)]
    [InlineData("Step", 2, 1, Direction.East)]
    [InlineData("Zigzag", 4, -2, Direction.East)]
    [InlineData("Rectangle2", 0, 0, Direction.East)]
    [InlineData("Staircase", 3, 3, Direction.East)]
    [InlineData("Random", 3, 10, Direction.South)]
    [InlineData("TwoSquares", 6, 0, Direction.East)]
    public void Example_EndsInExpectedState(string name, int x, int y, Direction direction)
    {
        var result = new ProgramExecutor().Execute(Find(name));

        Assert.Equal(x, result.FinalX);
        Assert.Equal(y, result.FinalY);
        Assert.Equal(direction, result.FinalDirection);
    }

    [Fact]
    public void Rectangle1AndRectangle2_GiveTheSameTrace()
    {
        var executor = new ProgramExecutor();

        Assert.Equal(
            executor.Execute(Find("Rectangle1")).FormatTrace(),
            executor.Execute(Find("Rectangle2")).FormatTrace());
    }

    [Theory]
    [InlineData(ExampleLevel.Basic)]
    [InlineData(ExampleLevel.Advanced)]
    [InlineData(ExampleLevel.Expert)]
    public void GetRandom_ReturnsAProgramOfThatLevel(ExampleLevel level)
    {
        var factory = new ExampleProgramFactory(new Random(42));
        var names = factory.Get(level).Select(p => p.Name).ToList();

        for (var i = 0; i < 20; i++)
        {
            Assert.Contains(factory.GetRandom(level).Name, names);
        }
    }

    [Fact]
    public void Get_UnknownLevel_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _factory.Get((ExampleLevel)99));
    }
}
