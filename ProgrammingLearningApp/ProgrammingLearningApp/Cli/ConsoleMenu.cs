using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Examples;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Metrics;
using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Cli;

//this is the commandline interface it only handles input and output it hands the real
// work to the factory, parser, executor and metrics calculator. this so a graphical
// interface can later replace this class without changing the rest of the app.
public sealed class ConsoleMenu
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly ExampleProgramFactory _examples;
    private readonly ProgramParser _parser;
    private readonly ProgramExecutor _executor;
    private readonly MetricsCalculator _metrics;

    public ConsoleMenu(TextReader input, TextWriter output)
        : this(input, output, new ExampleProgramFactory(), new ProgramParser(),
               new ProgramExecutor(), new MetricsCalculator())
    {
    }

    public ConsoleMenu(
        TextReader input,
        TextWriter output,
        ExampleProgramFactory examples,
        ProgramParser parser,
        ProgramExecutor executor,
        MetricsCalculator metrics)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(metrics);

        _input = input;
        _output = output;
        _examples = examples;
        _parser = parser;
        _executor = executor;
        _metrics = metrics;
    }

    public void Run()
    {
        _output.WriteLine("=== Programming Learning App ===");

        while (true)
        {
            var program = ChooseProgram();
            if (program is null)
            {
                break;
            }

            if (!WorkWithProgram(program))
            {
                break;
            }
        }

        _output.WriteLine("Goodbye!");
    }

    //returns the chosen program or null when the user quits
    private GridProgram? ChooseProgram()
    {
        while (true)
        {
            _output.WriteLine();
            _output.WriteLine("Step 1: choose a program");
            _output.WriteLine("  1. Random basic example");
            _output.WriteLine("  2. Random advanced example");
            _output.WriteLine("  3. Random expert example");
            _output.WriteLine("  4. Import a program from a text file");
            _output.WriteLine("  0. Quit");

            switch (Ask("> "))
            {
                case null or "0":
                    return null;
                case "1":
                    return LoadExample(ExampleLevel.Basic);
                case "2":
                    return LoadExample(ExampleLevel.Advanced);
                case "3":
                    return LoadExample(ExampleLevel.Expert);
                case "4":
                    var imported = Import();
                    if (imported is not null)
                    {
                        return imported;
                    }
                    break;
                default:
                    _output.WriteLine("Invalid choice, please enter one of the numbers above.");
                    break;
            }
        }
    }

    //returns false when input has ended
    private bool WorkWithProgram(GridProgram program)
    {
        while (true)
        {
            _output.WriteLine();
            _output.WriteLine($"Step 2: what do you want to do with '{program.Name}'?");
            _output.WriteLine("  1. Execute the program");
            _output.WriteLine("  2. Calculate metrics");
            _output.WriteLine("  0. Back to step 1");

            switch (Ask("> "))
            {
                case null:
                    return false;
                case "0":
                    return true;
                case "1":
                    _output.WriteLine(_executor.Execute(program));
                    break;
                case "2":
                    WriteMetrics(_metrics.Calculate(program));
                    break;
                default:
                    _output.WriteLine("Invalid choice, please enter one of the numbers above.");
                    break;
            }
        }
    }

    private GridProgram LoadExample(ExampleLevel level)
    {
        var program = _examples.GetRandom(level);
        _output.WriteLine($"Loaded {level.ToString().ToLowerInvariant()} example '{program.Name}'.");
        return program;
    }

    private GridProgram? Import()
    {
        var path = Ask("Path to the program file: ");
        if (path is null)
        {
            return null;
        }

        try
        {
            var program = _parser.ParseFile(CleanPath(path));
            _output.WriteLine($"Imported program '{program.Name}'.");
            return program;
        }
        catch (Exception e) when (e is FormatException or IOException
                                      or UnauthorizedAccessException or ArgumentException)
        {
            _output.WriteLine($"Could not import the file: {e.Message}");
            return null;
        }
    }

    private void WriteMetrics(ProgramMetrics metrics)
    {
        _output.WriteLine($"No. of commands: {metrics.CommandCount}");
        _output.WriteLine($"Max nesting: {metrics.MaxNestingLevel}");
        _output.WriteLine($"No. of repeats: {metrics.RepeatCount}");
    }

    //returns the trmmed input line or null when there is no more input
    private string? Ask(string prompt)
    {
        _output.Write(prompt);
        var line = _input.ReadLine();

        if (line is null)
        {
            _output.WriteLine();
            return null;
        }

        return line.Trim();
    }

    //accepts paths as typed or dragged into the terminal.
    private static string CleanPath(string path)
    {
        path = path.Trim();

        if (path.Length >= 2 && (path[0] == '"' || path[0] == '\'') && path[^1] == path[0])
        {
            path = path[1..^1];
        }

        path = path.Replace("\\ ", " ");

        if (path.StartsWith("~/"))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path[2..]);
        }

        return path;
    }
}
