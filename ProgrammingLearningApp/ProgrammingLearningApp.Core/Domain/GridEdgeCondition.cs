namespace ProgrammingLearningApp.Domain;

//true when the cell directly in front of the character is outside the grid
//without a grid there is no edge, so the answer is false
public sealed class GridEdgeCondition : Condition
{
    public override string Name => "GridEdge";

    public override bool IsTrue(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (state.Grid is null)
        {
            return false;
        }

        var target = state.Character.Position.Step(state.Character.Direction);
        return !state.Grid.Contains(target);
    }
}