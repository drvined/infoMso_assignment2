using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Metrics;
public sealed class MetricsCalculator
{
    public ProgramMetrics Calculate(GridProgram program)
    {
        var counter = new Counter();
        Visit(program.Commands, nestingLevel: 0, counter);

        return new ProgramMetrics(counter.Commands, counter.MaxNesting, counter.Repeats);
    }
    private static void Visit(IEnumerable<Command> commands, int nestingLevel, Counter counter)
    {
        foreach (Command command in commands)
        {
            counter.Commands++;
            counter.MaxNesting = Math.Max(counter.MaxNesting, nestingLevel);

            if (command is Repeat repeat)
            {
                counter.Repeats++;
                Visit(repeat.Commands, nestingLevel + 1, counter);
            }
        }
    }

    private sealed class Counter
    {
        public int Commands { get; set; }
        public int Repeats { get; set; }
        public int MaxNesting { get; set; }
    }
}