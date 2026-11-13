using ProgrammingLearningApp.Exercises;

namespace ProgrammingLearningApp.Tests;

//a fake file reader: the files live in a dictionary, so the tests do not touch the disk
public sealed class FakeFileReader : IFileReader
{
    private readonly Dictionary<string, string> _files;

    public FakeFileReader(Dictionary<string, string> files)
    {
        _files = files;
    }

    public bool Exists(string path) => _files.ContainsKey(path);

    public string ReadAllText(string path) => _files[path];
}
