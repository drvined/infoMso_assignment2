namespace ProgrammingLearningApp.Domain;

//true when the cell directly in front of character is a wall
//without grid there are no walls, so answer is false
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