using System.Text.Json;
using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Export;

//writes a program as json
public sealed class JsonProgramExporter : IProgramExporter
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string FormatName => "JSON (.json)";

    public string FileExtension => ".json";

    public string Export(GridProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        return JsonSerializer.Serialize(
            new { name = program.Name, commands = ToJson(program.Commands) },
            Options);
    }

    private static List<object> ToJson(IEnumerable<Command> commands)
    {
        var result = new List<object>();

        foreach (var command in commands)
        {
            object item = command switch
            {
                Move move => new { type = "Move", steps = move.Steps },
                Turn turn => new { type = "Turn", side = turn.Side == TurnSide.Left ? "left" : "right" },
                Repeat repeat => new { type = "Repeat", times = repeat.Times, commands = ToJson(repeat.Commands) },
                RepeatUntil until => new
                {
                    type = "RepeatUntil",
                    condition = until.Condition.Name,
                    commands = ToJson(until.Commands)
                },
                _ => throw new NotSupportedException($"Cannot write a command of type {command.GetType().Name}.")
            };

            result.Add(item);
        }

        return result;
    }
}
