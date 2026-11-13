using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Exercises;

//reads a pathfinding exercise from text file
public sealed class ExerciseLoader
{
    private readonly GridParser _gridParser = new();
    private readonly IFileReader _fileReader;

    // reader is a parameter so tests can pass a fake
    public ExerciseLoader(IFileReader? fileReader = null)
    {
        _fileReader = fileReader ?? new FileSystemFileReader();
    }

    public PathfindingExercise Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!_fileReader.Exists(path))
        {
            throw new FileNotFoundException($"Exercise file not found: {path}", path);
        }

        var gridText = _fileReader.ReadAllText(path);

        _gridParser.Parse(gridText);

        var name = Path.GetFileNameWithoutExtension(path);
        return new PathfindingExercise(name, $"Bereik de x in de oefening '{name}'.", gridText);
    }
}
