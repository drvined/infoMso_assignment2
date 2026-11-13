using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Metrics;

//calculates the metrics number of commands, max nesting and number of repeats
//every LoopCommand (Repeat and RepeatUntil) counts as a repeat, so a new kind of loop is counted without changing this class
public sealed class MetricsCalculator
{
    public ProgramMetrics Calculate(GridProgram program)
    {
        var counter = new Counter();
        CountCommands(program.Commands, nestingLevel: 0, counter);

        return new ProgramMetrics(counter.Commands, counter.MaxNesting, counter.Repeats);
    }

    private static void CountCommands(IEnumerable<Command> commands, int nestingLevel, Counter counter)
    {
        foreach (var command in commands)
        {
            counter.Commands++;
            counter.MaxNesting = Math.Max(counter.MaxNesting, nestingLevel);

            if (command is LoopCommand loop)
            {
                counter.Repeats++;
                CountCommands(loop.Commands, nestingLevel + 1, counter);
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
