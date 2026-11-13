namespace ProgrammingLearningApp.Domain;

//true when the cell directly in front of the character is a wall
//without a grid there are no walls, so the answer is false
public sealed class WallAheadCondition : Condition
{
    public override bool IsTrue(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (state.Grid is null)
        {
            return false;
        }

        var target = state.Character.Position.Step(state.Character.Direction);
        return state.Grid.IsBlocked(target);
    }
}