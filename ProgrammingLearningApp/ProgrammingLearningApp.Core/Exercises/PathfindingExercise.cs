using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Exercises;

//exercise where the student writes program that brings the character from s to x
//the grid text is parsed once here, so the loader, the checker and the ui all use the same Grid
//an invalid grid text throws a FormatException when the exercise is created
public sealed record PathfindingExercise(string Name, string Description, string GridText)
{
    public Grid Grid { get; } = new GridParser().Parse(GridText);
}