using ProgrammingLearningApp.Exercises;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ExerciseLoaderTests
{
    [Fact]
    public void Load_UsesFileNameAsName_AndFileContentAsGrid()
    {
        var directory = CreateTempDirectory();
        try
        {
            var path = Path.Combine(directory, "recht_vooruit.txt");
            File.WriteAllText(path, "soox");

            var exercise = new ExerciseLoader().Load(path);

            Assert.Equal("recht_vooruit", exercise.Name);
            Assert.Equal("soox", exercise.GridText);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Load_InvalidGridOnDisk_ThrowsFormatException()
    {
        var directory = CreateTempDirectory();
        try
        {
            var path = Path.Combine(directory, "kapot.txt");
            File.WriteAllText(path, "s?x");

            Assert.Throws<FormatException>(() => new ExerciseLoader().Load(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    //with a fake reader the loader is tested without touching the disk
    [Fact]
    public void Load_WithFakeReader_ReturnsTheExercise()
    {
        var reader = new FakeFileReader(new() { ["oefeningen/soox.txt"] = "soox" });

        var exercise = new ExerciseLoader(reader).Load("oefeningen/soox.txt");

        Assert.Equal("soox", exercise.Name);
        Assert.Equal("soox", exercise.GridText);
    }

    [Fact]
    public void Load_WithFakeReaderAndNoFile_ThrowsFileNotFoundException()
    {
        var reader = new FakeFileReader(new());

        Assert.Throws<FileNotFoundException>(() => new ExerciseLoader(reader).Load("oefeningen/bestaat_niet.txt"));
    }

    [Fact]
    public void Load_WithFakeReaderAndBrokenGrid_ThrowsFormatException()
    {
        var reader = new FakeFileReader(new() { ["kapot.txt"] = "s?x" });

        Assert.Throws<FormatException>(() => new ExerciseLoader(reader).Load("kapot.txt"));
    }

    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        return directory;
    }
}
