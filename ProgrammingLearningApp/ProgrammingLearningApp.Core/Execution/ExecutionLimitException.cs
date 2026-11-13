namespace ProgrammingLearningApp.Execution;

//the program ran for too many steps, probably an endless loop
public class ExecutionLimitException : ProgramRuntimeException
{
    public ExecutionLimitException(string message) : base(message)
    {
    }
}