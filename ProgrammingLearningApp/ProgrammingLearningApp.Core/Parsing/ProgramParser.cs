using System.Globalization;
using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Parsing;

public sealed class ProgramParser
{
    private const int IndentSize = 4;

    //one non empty line of the source, with its indentation level
    private sealed record SourceLine(int Number, int Level, string Content);

    //reads and parses a program file. The program name is the file name without extension
    public GridProgram ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Program file not found: {path}", path);
        }

        var name = Path.GetFileNameWithoutExtension(path);
        return Parse(name, File.ReadAllText(path));
    }

    //parses program text into a program with the given name
    public GridProgram Parse(string name, string text)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(text);

        var lines = ReadLines(text);
        if (lines.Count == 0)
        {
            throw new FormatException("The program contains no commands.");
        }

        var index = 0;
        var commands = ParseBlock(lines, ref index, level: 0);
        return new GridProgram(name, commands);
    }

    private static List<SourceLine> ReadLines(string text)
    {
        var result = new List<SourceLine>();
        var rawLines = text.Split('\n');

        for (var i = 0; i < rawLines.Length; i++)
        {
            var lineNumber = i + 1;
            var line = rawLines[i].TrimEnd('\r', ' ');

            if (line.Length == 0)
            {
                continue;
            }

            var spaces = 0;
            while (spaces < line.Length && line[spaces] == ' ')
            {
                spaces++;
            }

            var content = line[spaces..];

            if (content.StartsWith('\t'))
            {
                throw Error(lineNumber, content, "Use 4 spaces for indentation, not tabs.");
            }

            if (spaces % IndentSize != 0)
            {
                throw Error(lineNumber, content,
                    $"Indentation must be a multiple of {IndentSize} spaces (found {spaces}).");
            }

            result.Add(new SourceLine(lineNumber, spaces / IndentSize, content));
        }

        return result;
    }


    //parses consecutive lines at exactly level, it stops
    //first line that is indented less 
    private static List<Command> ParseBlock(List<SourceLine> lines, ref int index, int level)
    {
        var commands = new List<Command>();

        while (index < lines.Count)
        {
            var line = lines[index];

            if (line.Level < level)
            {
                break;
            }

            if (line.Level > level)
            {
                throw Error(line, "Unexpected indentation.");
            }

            index++;
            commands.Add(ParseCommand(line, lines, ref index));
        }

        return commands;
    }

    private static Command ParseCommand(SourceLine line, List<SourceLine> lines, ref int index)
    {
        var parts = line.Content.Split(' ');

        switch (parts[0])
        {
            case "Move" when parts.Length == 2:
                return new Move(ParseCount(line, parts[1]));

            case "Move":
                throw Error(line, "Expected 'Move [x]', for example 'Move 10'.");

            case "Turn" when parts.Length == 2 && parts[1] == "left":
                return new Turn(TurnSide.Left);

            case "Turn" when parts.Length == 2 && parts[1] == "right":
                return new Turn(TurnSide.Right);

            case "Turn":
                throw Error(line, "Expected 'Turn left' or 'Turn right'.");

            case "Repeat" when parts.Length == 3 && parts[2] == "times":
                var times = ParseCount(line, parts[1]);

                if (index >= lines.Count || lines[index].Level <= line.Level)
                {
                    throw Error(line,
                        $"A repeat needs at least one command below it, indented {IndentSize} more spaces.");
                }

                var body = ParseBlock(lines, ref index, line.Level + 1);
                return new Repeat(times, body);

            case "Repeat":
                throw Error(line, "Expected 'Repeat [x] times', for example 'Repeat 4 times'.");

            default:
                throw Error(line, $"Unknown command '{parts[0]}'. Valid commands are Move, Turn and Repeat.");
        }
    }

    //parses a whole number of zero or more
    private static int ParseCount(SourceLine line, string text)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            throw Error(line, $"'{text}' is not a valid number (expected a whole number of 0 or more).");
        }

        return value;
    }

    private static FormatException Error(SourceLine line, string message) =>
        Error(line.Number, line.Content, message);

    private static FormatException Error(int lineNumber, string content, string message) =>
        new($"Line {lineNumber}: {message} (\"{content}\")");
}
