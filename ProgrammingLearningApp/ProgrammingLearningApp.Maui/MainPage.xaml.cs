using Microsoft.Maui.Storage;
using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Examples;
using ProgrammingLearningApp.Export;
using ProgrammingLearningApp.Exercises;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Metrics;
using ProgrammingLearningApp.Parsing;
using DomainGrid = ProgrammingLearningApp.Domain.Grid;

namespace ProgrammingLearningApp.Maui;

public partial class MainPage : ContentPage
{
    private readonly ProgramParser _programParser = new();
    private readonly GridParser _gridParser = new();
    private readonly ProgramExecutor _executor = new();
    private readonly MetricsCalculator _metrics = new();
    private readonly ExampleProgramFactory _examples = new();
    private readonly ProgramWriter _programWriter = new();
    private readonly ExerciseLoader _exerciseLoader = new();
    private readonly ExerciseChecker _exerciseChecker = new();
    private readonly GridDrawable _drawable = new();
    private readonly List<IProgramExporter> _exporters = [new ProgramWriter(), new JsonProgramExporter()];
    private IReadOnlyList<GridProgram> _examplePrograms = [];
    private PathfindingExercise? _currentExercise;

    public MainPage()
    {
        InitializeComponent();
        GridView.Drawable = _drawable;

        _examplePrograms = _examples.GetAll();
        ExamplePicker.ItemsSource = _examplePrograms.Select(p => p.Name).ToList();
        LevelPicker.ItemsSource = Enum.GetValues<ExampleLevel>().Select(l => l.ToString()).ToList();
        ExportFormatPicker.ItemsSource = _exporters.Select(e => e.FormatName).ToList();
    }

    //opens a .txt program file, checks it and puts the text in the program editor
    private async void OnImportClicked(object? sender, EventArgs e)
    {
        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Kies een programmabestand"
        });

        if (file is null)
        {
            return;
        }

        if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            ResultLabel.Text = "Kies een .txt bestand.";
            return;
        }

        try
        {
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();

            //checks the file now, so a mistake shows up at import and not only when the program runs
            var name = Path.GetFileNameWithoutExtension(file.FileName);
            var program = _programParser.Parse(name, text);

            ProgramEditor.Text = text;
            ResultLabel.Text = $"Programma '{program.Name}' is geimporteerd.";
        }
        catch (FormatException ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    //saves the program in the editor in the chosen format to the Documents folder
    private void OnExportClicked(object? sender, EventArgs e)
    {
        if (ExportFormatPicker.SelectedIndex < 0)
        {
            ResultLabel.Text = "Kies eerst een exportformaat.";
            return;
        }

        try
        {
            var program = _programParser.Parse("programma", ProgramEditor.Text ?? string.Empty);
            var exporter = _exporters[ExportFormatPicker.SelectedIndex];

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(folder, program.Name + exporter.FileExtension);

            File.WriteAllText(path, exporter.Export(program));
            ResultLabel.Text = $"Geexporteerd naar: {path}";
        }
        catch (FormatException ex)
        {
            ResultLabel.Text = ex.Message;
        }
        catch (IOException ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    private void OnRunClicked(object? sender, EventArgs e)
    {
        try
        {
            var program = _programParser.Parse("program", ProgramEditor.Text ?? string.Empty);
            RunProgram(program);
        }
        catch (FormatException ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    private void OnRunExampleClicked(object? sender, EventArgs e)
    {
        if (ExamplePicker.SelectedIndex < 0)
        {
            ResultLabel.Text = "Kies eerst een voorbeeld.";
            return;
        }

        RunProgram(_examplePrograms[ExamplePicker.SelectedIndex]);
    }

    //loads a random program of the chosen level into the program editor
    private void OnLoadRandomClicked(object? sender, EventArgs e)
    {
        if (LevelPicker.SelectedIndex < 0)
        {
            ResultLabel.Text = "Kies eerst een niveau.";
            return;
        }

        var level = Enum.GetValues<ExampleLevel>()[LevelPicker.SelectedIndex];
        var program = _examples.GetRandom(level);

        ProgramEditor.Text = _programWriter.ToText(program);
        ResultLabel.Text = $"Willekeurig {level} programma geladen: {program.Name}.";
    }

    //opens an exercise file and shows its grid and description
    private async void OnLoadExerciseClicked(object? sender, EventArgs e)
    {
        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Kies een oefeningbestand"
        });

        if (file is null)
        {
            return;
        }

        try
        {
            _currentExercise = _exerciseLoader.Load(file.FullPath);

            GridEditor.Text = _currentExercise.GridText;
            ExerciseDescriptionLabel.Text = _currentExercise.Description;
            ExerciseLabel.Text = string.Empty;
        }
        catch (FormatException ex)
        {
            ExerciseLabel.Text = ex.Message;
        }
        catch (IOException ex)
        {
            ExerciseLabel.Text = ex.Message;
        }
    }

    //runs the program in the editor on the grid of the loaded exercise and checks if it is solved
    private void OnCheckExerciseClicked(object? sender, EventArgs e)
    {
        if (_currentExercise is null)
        {
            ExerciseLabel.Text = "Laad eerst een oefening.";
            return;
        }

        try
        {
            var program = _programParser.Parse("oefening", ProgramEditor.Text ?? string.Empty);

            GridEditor.Text = _currentExercise.GridText;
            RunProgram(program);

            ExerciseLabel.Text = _exerciseChecker.IsSolved(program, _currentExercise)
                ? $"Opgelost: '{_currentExercise.Name}' is gehaald."
                : "Nog niet opgelost: de x is niet bereikt zonder tegen een muur of de rand te lopen.";
        }
        catch (FormatException ex)
        {
            ExerciseLabel.Text = ex.Message;
        }
    }

    //runs a program on the grid that is typed in the grid editor and shows the result and metrics
    //the path is drawn also when the program stops with an error, so the user sees how far the character got
    private void RunProgram(GridProgram program)
    {
        DomainGrid? grid = null;
        IReadOnlyList<Position> path = [];

        try
        {
            if (string.IsNullOrWhiteSpace(GridEditor.Text))
            {
                ResultLabel.Text = "Vul eerst een grid in.";
            }
            else
            {
                grid = _gridParser.Parse(GridEditor.Text);

                var start = FindStart(grid);
                var character = new Character(start.X, start.Y, Direction.East);

                var result = _executor.Execute(program, grid, character);
                path = result.Path;
                ResultLabel.Text = result.ToString();
            }
        }
        catch (FormatException ex)
        {
            ResultLabel.Text = ex.Message;
        }
        catch (ProgramRuntimeException ex)
        {
            path = ex.Path;
            ResultLabel.Text = $"Fout: {ex.Message}";
        }

        if (grid is not null)
        {
            _drawable.Update(grid, path);
            GridView.Invalidate();
        }

        var metrics = _metrics.Calculate(program);
        MetricsLabel.Text = $"Commando's: {metrics.CommandCount}, " +
                            $"maximale nesting: {metrics.MaxNestingLevel}, " +
                            $"herhalingen: {metrics.RepeatCount}";
    }

    //the start cell (s) in the grid, or (0,0) when the grid has none
    private static Position FindStart(DomainGrid grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if (grid.GetCell(new Position(x, y)) == CellType.Start)
                {
                    return new Position(x, y);
                }
            }
        }

        return new Position(0, 0);
    }
}
