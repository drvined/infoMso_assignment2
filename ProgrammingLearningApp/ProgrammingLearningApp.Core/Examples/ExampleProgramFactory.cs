using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Examples;

public enum ExampleLevel
{
    Basic,     // only moves and turns
    Advanced,  // uses repeat
    Expert     // uses nested repeats
}

//factory that builds the hardcoded example programs directly as objects
public sealed class ExampleProgramFactory
{
    private readonly Random _random;

    public ExampleProgramFactory() : this(Random.Shared)
    {
    }

    public ExampleProgramFactory(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        _random = random;
    }

    public IReadOnlyList<GridProgram> GetAll() =>
        Enum.GetValues<ExampleLevel>().SelectMany(Get).ToList();

    public IReadOnlyList<GridProgram> Get(ExampleLevel level) => level switch
    {
        ExampleLevel.Basic => CreateBasic(),
        ExampleLevel.Advanced => CreateAdvanced(),
        ExampleLevel.Expert => CreateExpert(),
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown example level.")
    };

    public GridProgram GetRandom(ExampleLevel level)
    {
        var programs = Get(level);
        return programs[_random.Next(programs.Count)];
    }

    private static List<GridProgram> CreateBasic() =>
    [
        new GridProgram("Rectangle1",
        [
            new Move(10), TurnRight(),
            new Move(10), TurnRight(),
            new Move(10), TurnRight(),
            new Move(10), TurnRight()
        ]),

        new GridProgram("Step",
        [
            new Move(1), TurnRight(),
            new Move(1), TurnLeft(),
            new Move(1)
        ]),

        new GridProgram("Zigzag",
        [
            new Move(2), TurnLeft(),
            new Move(2), TurnRight(),
            new Move(2)
        ])
    ];

    private static List<GridProgram> CreateAdvanced() =>
    [
        new GridProgram("Rectangle2",
        [
            new Repeat(4, [new Move(10), TurnRight()])
        ]),

        new GridProgram("Staircase",
        [
            new Repeat(3, [new Move(1), TurnRight(), new Move(1), TurnLeft()])
        ]),

        new GridProgram("FindExit3",
        [
            new RepeatUntil(new WallAheadCondition(), [new Move(10)]),
            TurnRight()
        ])
    ];

    private static List<GridProgram> CreateExpert() =>
    [
        new GridProgram("Random",
        [
            new Move(5), TurnLeft(), TurnLeft(),
            new Move(3), TurnRight(),
            new Repeat(3,
            [
                new Move(1), TurnRight(),
                new Repeat(5, [new Move(2)])
            ]),
            TurnLeft()
        ]),

        new GridProgram("TwoSquares",
        [
            new Repeat(2,
            [
                new Repeat(4, [new Move(2), TurnRight()]),
                new Move(3)
            ])
        ]),

        new GridProgram("FindExit1",
        [
            new Repeat(3,
            [
                new RepeatUntil(new WallAheadCondition(), [new Move(1)]),
                TurnRight()
            ])
        ]),

        new GridProgram("FindExit2",
        [
            new Repeat(2,
            [
                new RepeatUntil(new GridEdgeCondition(), [new Move(1)]),
                TurnRight()
            ])
        ])
    ];

    private static Turn TurnLeft() => new(TurnSide.Left);

    private static Turn TurnRight() => new(TurnSide.Right);
}
