using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Tests;

internal static class TestPrograms
{
    public static string Join(params string[] lines) => string.Join("\n", lines);

    public static readonly string Example1 = Join(
        "Move 10", "Turn right", "Move 10", "Turn right",
        "Move 10", "Turn right", "Move 10", "Turn right");

    public static readonly string Example2 = Join(
        "Repeat 4 times",
        "    Move 10",
        "    Turn right");

    public static readonly string Example3 = Join(
        "Move 5",
        "Turn left",
        "Turn left",
        "Move 3",
        "Turn right",
        "Repeat 3 times",
        "    Move 1",
        "    Turn right",
        "    Repeat 5 times",
        "        Move 2",
        "Turn left");

    public static GridProgram Parse(string text) => new ProgramParser().Parse("test", text);
}