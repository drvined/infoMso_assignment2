using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ProgramParserFileTests
{
    private static string CreateFile(string fileName, string contents)
    {
        var folder = Path.Combine(Path.GetTempPath(), $"parser-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);
        File.WriteAllText(path, contents);
        return path;
    }

    private static void DeleteFile(string path) =>
        Directory.Delete(Path.GetDirectoryName(path)!, recursive: true);

    [Fact]
    public void ParseFile_UsesFileNameAsProgramName()
    {
        var path = CreateFile("Rectangle2.txt", TestPrograms.Example2);
        try
        {
            GridProgram program = new ProgramParser().ParseFile(path);

            Assert.Equal("Rectangle2", program.Name);
            Assert.IsType<Repeat>(Assert.Single(program.Commands));
        }
        finally
        {
            DeleteFile(path);
        }
    }

    [Fact]
    public void ParseFile_WindowsLineEndings_AreAccepted()
    {
        var path = CreateFile("Windows.txt", TestPrograms.Example3.Replace("\n", "\r\n"));
        try
        {
            GridProgram program = new ProgramParser().ParseFile(path);

            Assert.Equal(7, program.Commands.Count);
        }
        finally
        {
            DeleteFile(path);
        }
    }

    [Fact]
    public void ParseFile_WithFakeReader_ReadsFromTheReader()
    {
        var reader = new FakeFileReader(new() { ["programmas/Vierkant.txt"] = TestPrograms.Example2 });

        GridProgram program = new ProgramParser(reader).ParseFile("programmas/Vierkant.txt");

        Assert.Equal("Vierkant", program.Name);
        Assert.IsType<Repeat>(Assert.Single(program.Commands));
    }

    [Fact]
    public void ParseFile_WithFakeReaderAndNoFile_Throws()
    {
        var reader = new FakeFileReader(new());

        Assert.Throws<FileNotFoundException>(() => new ProgramParser(reader).ParseFile("programmas/bestaat_niet.txt"));
    }

    [Fact]
    public void ParseFile_MissingFile_Throws()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new ProgramParser().ParseFile("/this/file/does/not/exist.txt"));
    }
}
