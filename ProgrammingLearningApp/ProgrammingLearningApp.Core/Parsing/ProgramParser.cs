using System.Globalization;
using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Parsing;

public sealed class ProgramParser
{
    private const int IndentSize = 4;

    private sealed record SourceLine(int Number, int Level, string Content);

    //reads and parses program file
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

    //parses program text into program with the given name
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
        var rawLines = TextLines.Split(text);

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


    //parses consecutive lines at exactly level
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

    //picks the method for the keyword at the start of the line, each command is parsed in its own method
    private static Command ParseCommand(SourceLine line, List<SourceLine> lines, ref int index)
    {
        var parts = line.Content.Split(' ');

        switch (parts[0])
        {
            case "Move":
                return ParseMove(line, parts);

            case "Turn":
                return ParseTurn(line, parts);

            case "Repeat":
                return ParseRepeat(line, parts, lines, ref index);

            case "RepeatUntil":
                return ParseRepeatUntil(line, parts, lines, ref index);

            default:
                throw Error(line,
                    $"Unknown command '{parts[0]}'. Valid commands are Move, Turn, Repeat and RepeatUntil.");
        }
    }

    //Move [x]
    private static Move ParseMove(SourceLine line, string[] parts) =>
        parts.Length == 2
            ? new Move(ParseCount(line, parts[1]))
            : throw Error(line, "Expected 'Move [x]', for example 'Move 10'.");

    //Turn left or Turn right
    private static Turn ParseTurn(SourceLine line, string[] parts) => parts switch
    {
        [_, "left"] => new Turn(TurnSide.Left),
        [_, "right"] => new Turn(TurnSide.Right),
        _ => throw Error(line, "Expected 'Turn left' or 'Turn right'.")
    };

    //Repeat [x] or Repeat [x] times, and Repeat until [condition] as another way to write RepeatUntil
    private static Command ParseRepeat(SourceLine line, string[] parts, List<SourceLine> lines, ref int index)
    {
        if (parts.Length == 2 || (parts.Length == 3 && parts[2] == "times"))
        {
            return new Repeat(ParseCount(line, parts[1]), ParseBody(line, lines, ref index));
        }

        if (parts.Length == 3 && parts[1] == "until")
        {
            return new RepeatUntil(ParseCondition(line, parts[2]), ParseBody(line, lines, ref index));
        }

        throw Error(line, "Expected 'Repeat [x]', for example 'Repeat 4'.");
    }

    //RepeatUntil [condition]
    private static RepeatUntil ParseRepeatUntil(SourceLine line, string[] parts, List<SourceLine> lines, ref int index)
    {
        if (parts.Length != 2)
        {
            throw Error(line, "Expected 'RepeatUntil [condition]', for example 'RepeatUntil WallAhead'.");
        }

        return new RepeatUntil(ParseCondition(line, parts[1]), ParseBody(line, lines, ref index));
    }

    private static List<Command> ParseBody(SourceLine line, List<SourceLine> lines, ref int index)
    {
        if (index >= lines.Count || lines[index].Level <= line.Level)
        {
            throw Error(line, $"A repeat needs at least one command below it, indented {IndentSize} more spaces.");
        }

        return ParseBlock(lines, ref index, line.Level + 1);
    }

    //parses a whole number
    private static int ParseCount(SourceLine line, string text)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            throw Error(line, $"'{text}' is not a valid number (expected a whole number of 0 or more).");
        }

        return value;
    }

    //read name of a condition
    private static Condition ParseCondition(SourceLine line, string text) => text switch
    {
        "WallAhead" => new WallAheadCondition(),
        "GridEdge" => new GridEdgeCondition(),
        _ => throw Error(line, $"Unknown condition '{text}'. Valid conditions are WallAhead and GridEdge."),
    };

    private static FormatException Error(SourceLine line, string message) =>
        Error(line.Number, line.Content, message);

    private static FormatException Error(int lineNumber, string content, string message) =>
        new($"Line {lineNumber}: {message} (\"{content}\")");
}