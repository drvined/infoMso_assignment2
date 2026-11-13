namespace ProgrammingLearningApp.IO;

//reads text files, loader uses this instead so tests can use a fake
public interface IFileReader
{
    bool Exists(string path);

    string ReadAllText(string path);
}
