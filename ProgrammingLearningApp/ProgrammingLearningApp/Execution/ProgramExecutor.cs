using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;



public sealed class ProgramExecutor
{
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    public ExecutionResult Execute(GridProgram program, Character character)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(character);

        var trace = new List<string>();
        ExecuteCommands(program.Commands, character, trace);

        return new ExecutionResult(trace, character.X, character.Y, character.Direction);
    }

    private static void ExecuteCommands(IEnumerable<Command> commands, Character character, List<string> trace)
    {
        foreach (var command in commands)
        {
            ExecuteCommand(command, character, trace);
        }
    }

    private static void ExecuteCommand(Command command, Character character, List<string> trace)
    {
        switch (command)
        {
            case Move move:
                for (var i = 0; i < move.Steps; i++)
                {
                    character.StepForward();
                }
                trace.Add($"Move {move.Steps}");
                break;

            case Turn turn:
                if (turn.Side == TurnSide.Left)
                {
                    character.TurnLeft();
                    trace.Add("Turn left");
                }
                else
                {
                    character.TurnRight();
                    trace.Add("Turn right");
                }
                break;

            case Repeat repeat:
                for (var i = 0; i < repeat.Times; i++)
                {
                    ExecuteCommands(repeat.Commands, character, trace);
                }
                break;

            default:
                throw new NotSupportedException($"Unknown command type: {command.GetType().Name}");
        }
    }
}
