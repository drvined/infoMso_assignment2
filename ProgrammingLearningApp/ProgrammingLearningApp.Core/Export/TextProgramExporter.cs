using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Export;

//exports a program in the text format, the same format the parser reads (so export and import match)
public sealed class TextProgramExporter : IProgramExporter
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
                    lines.Add($"{indent}RepeatUntil {repeatUntil.Condition.Name}");
                    WriteCommands(repeatUntil.Commands, level + 1, lines);
                    break;

                default:
                    throw new NotSupportedException($"Cannot write a command of type {command.GetType().Name}.");
            }
        }
    }
}
