namespace ProgrammingLearningApp.Domain;

//base class for errors that happen while a program is running
public class ProgramRuntimeException : Exception
{
    public ProgramRuntimeException(string message) : base(message)
    {
    }
}