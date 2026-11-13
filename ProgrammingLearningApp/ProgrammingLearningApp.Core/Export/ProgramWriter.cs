using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Export;

//writes a program back to the text format, so it can be saved and read again with ProgramParser
public sealed class ProgramWriter : IProgramExporter
{
    private const string Indent = "    ";

    public string FormatName => "Tekst (.txt)";

    public string FileExtension => ".txt";

    public string Export(GridProgram program) => ToText(program);

    public string ToText(GridProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        var lines = new List<string>();
        WriteCommands(program.Commands, 0, lines);

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    //the name of a condition as it is written in the program text, also used by the JSON exporter
    internal static string ConditionName(Condition condition) => condition switch
    {
        WallAheadCondition => "WallAhead",
        GridEdgeCondition => "GridEdge",
        _ => throw new NotSupportedException($"Cannot write a condition of type {condition.GetType().Name}.")
    };

    //one line per command, the commands inside a repeat are indented one level deeper
    private static void WriteCommands(IEnumerable<Command> commands, int level, List<string> lines)
    {
        var indent = string.Concat(Enumerable.Repeat(Indent, level));

        foreach (var command in commands)
        {
            switch (command)
            {
                case Move move:
                    lines.Add($"{indent}Move {move.Steps}");
                    break;

                case Turn turn:
                    lines.Add($"{indent}Turn {(turn.Side == TurnSide.Left ? "left" : "right")}");
                    break;

                case Repeat repeat:
                    lines.Add($"{indent}Repeat {repeat.Times} times");
                    WriteCommands(repeat.Commands, level + 1, lines);
                    break;

                case RepeatUntil repeatUntil:
                    lines.Add($"{indent}Repeat until {ConditionName(repeatUntil.Condition)}");
                    WriteCommands(repeatUntil.Commands, level + 1, lines);
                    break;

                default:
                    throw new NotSupportedException($"Cannot write a command of type {command.GetType().Name}.");
            }
        }
    }
}
