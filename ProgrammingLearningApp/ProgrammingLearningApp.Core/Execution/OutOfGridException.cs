namespace ProgrammingLearningApp.Execution;

//the character tried to walk outside the grid
public class OutOfGridException : ProgramRuntimeException
{
    public OutOfGridException(string message) : base(message)
    {
    }
}