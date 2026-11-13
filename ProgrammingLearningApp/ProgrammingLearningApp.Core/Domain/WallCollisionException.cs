namespace ProgrammingLearningApp.Domain;

//the character tried to walk into a blocked cell
public class WallCollisionException : ProgramRuntimeException
{
    public WallCollisionException(string message) : base(message)
    {
    }
}