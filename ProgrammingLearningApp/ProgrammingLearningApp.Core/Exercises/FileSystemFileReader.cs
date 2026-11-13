namespace ProgrammingLearningApp.Exercises;

//the real reader, it reads from the disk
public sealed class FileSystemFileReader : IFileReader
{
    public bool Exists(string path) => File.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);
}
