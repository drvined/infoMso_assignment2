namespace ProgrammingLearningApp.Domain;

//the character tried to walk outside the grid
public class OutOfGridException : ProgramRuntimeException
{
    public OutOfGridException(string message) : base(message)
    {
    }
}