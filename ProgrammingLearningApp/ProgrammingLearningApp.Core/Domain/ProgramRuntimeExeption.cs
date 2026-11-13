namespace ProgrammingLearningApp.Domain;

//base class for errors that happen while a program is running
public class ProgramRuntimeException : Exception
{
    public ProgramRuntimeException(string message) : base(message)
    {
    }

    //the cells the character walked over before the error happened, so the error can still be drawn
    public IReadOnlyList<Position> Path { get; internal set; } = [];
}
