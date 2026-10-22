using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//runs a program on a character and keeps a trace of every move and turn that happened
public sealed class ProgramExecutor
{
    //runs the program on a new character that starts at (0,0) facing east
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    //runs the program on a character, that character gets changed
    //at the end the trace and where the character ended up go into ExecutionResult
    public ExecutionResult Execute(GridProgram program, Character character)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(character);

        var trace = new List<string>();
        ExecuteCommands(program.Commands, character, trace);

        return new ExecutionResult(trace, character.X, character.Y, character.Direction);
    }

    //runs a list of commands one after the other
    private static void ExecuteCommands(IEnumerable<Command> commands, Character character, List<string> trace)
    {
        foreach (var command in commands)
        {
            ExecuteCommand(command, character, trace);
        }
    }

    //runs one command
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

            //repeat runs its own commands again and again
            case Repeat repeat:
                for (var i = 0; i < repeat.Times; i++)
                {
                    ExecuteCommands(repeat.Commands, character, trace);
                }
                break;

            //only happens if someone adds a new command type and forgets to handle it here
            default:
                throw new NotSupportedException($"Unknown command type: {command.GetType().Name}");
        }
    }
}
