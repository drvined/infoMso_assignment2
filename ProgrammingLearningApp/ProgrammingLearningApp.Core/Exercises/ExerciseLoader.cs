using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Exercises;

//reads a pathfinding exercise from a text file: the content of the file is the grid
//and the name of the exercise is the file name without extension
public sealed class ExerciseLoader
{
    private readonly GridParser _gridParser = new();
    private readonly IFileReader _fileReader;

    //the reader is a parameter so tests can pass a fake; by default the disk is used
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

        //parses the grid once, so a broken file is reported when it is loaded and not when a program runs
        _gridParser.Parse(gridText);

        var name = Path.GetFileNameWithoutExtension(path);
        return new PathfindingExercise(name, $"Bereik de x in de oefening '{name}'.", gridText);
    }
}
