namespace ProgrammingLearningApp.Exercises;

//reads text files. The loader uses this instead of the file system directly, so tests can use a fake
public interface IFileReader
{
    bool Exists(string path);

    string ReadAllText(string path);
}
