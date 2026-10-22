using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;


//runs a GridProgram on a Character and records every executed Move and Turn.

public sealed class ProgramExecutor
{
    //runs program on a new character that starts at (0,0) facing east
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    //runs program on the given character (its state is changed)
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
