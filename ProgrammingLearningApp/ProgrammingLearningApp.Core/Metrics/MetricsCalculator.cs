using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Metrics;

//calculates the metrics from the assignment: number of commands, max nesting and number of repeats
//a repeat and a repeat until both count as a repeat
public sealed class MetricsCalculator
{
    //walks through the whole program once and puts the 3 numbers in a ProgramMetrics
    public ProgramMetrics Calculate(GridProgram program)
    {
        var counter = new Counter();
        CountCommands(program.Commands, nestingLevel: 0, counter);

        return new ProgramMetrics(counter.Commands, counter.MaxNesting, counter.Repeats);
    }

    //goes over every command at this level, when it finds a repeat it calls itself again one level deeper
    private static void CountCommands(IEnumerable<Command> commands, int nestingLevel, Counter counter)
    {
        foreach (var command in commands)
        {
            counter.Commands++;
            counter.MaxNesting = Math.Max(counter.MaxNesting, nestingLevel);

            switch (command)
            {
                case Repeat repeat:
                    counter.Repeats++;
                    CountCommands(repeat.Commands, nestingLevel + 1, counter);
                    break;

                case RepeatUntil repeatUntil:
                    counter.Repeats++;
                    CountCommands(repeatUntil.Commands, nestingLevel + 1, counter);
                    break;
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
