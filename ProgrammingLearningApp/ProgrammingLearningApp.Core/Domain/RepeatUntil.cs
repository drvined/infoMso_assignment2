namespace ProgrammingLearningApp.Domain;

//the repeat until command runs its commands again and again until the condition is true
//the condition is checked before every round, so when it is already true the commands never run
public sealed class RepeatUntil : LoopCommand
{
    public Condition Condition { get; }

    public RepeatUntil(Condition condition, IEnumerable<Command> commands) : base(commands)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    public override void Execute(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        while (!Condition.IsTrue(state))
        {
            state.CountStep();
            ExecuteBody(state);
        }
    }
}
