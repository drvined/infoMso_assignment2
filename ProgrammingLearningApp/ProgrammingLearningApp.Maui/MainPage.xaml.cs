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

//the main screen only reads input calls the Core classes and shows their results. THe logic lives in Core
public partial class MainPage : ContentPage
{
    private static readonly Color NeutralColor = Color.FromArgb("#ECECF1");
    private static readonly Color SuccessColor = Color.FromArgb("#3DDC84");
    private static readonly Color FailureColor = Color.FromArgb("#FFB347");
    private static readonly Color ErrorColor = Color.FromArgb("#FF6B6B");

    private readonly ProgramParser _programParser = new();
    private readonly GridParser _gridParser = new();
    private readonly ProgramExecutor _executor = new();
    private readonly MetricsCalculator _metrics = new();
    private readonly ExampleProgramFactory _examples = new();
    private readonly TextProgramExporter _textExporter = new();
    private readonly ExerciseLoader _exerciseLoader = new();
    private readonly ExerciseChecker _exerciseChecker = new();
    private readonly GridDrawable _drawable = new();
    private readonly List<IProgramExporter> _exporters = [new TextProgramExporter(), new JsonProgramExporter()];
    private readonly IReadOnlyList<GridProgram> _examplePrograms;

    public MainPage()
    {
        InitializeComponent();
        GridView.Drawable = _drawable;

        _examplePrograms = _examples.GetAll();
        ExamplePicker.ItemsSource = _examplePrograms.Select(p => p.Name).ToList();
        LevelPicker.ItemsSource = Enum.GetValues<ExampleLevel>().Select(l => l.ToString()).ToList();
        ExportFormatPicker.ItemsSource = _exporters.Select(e => e.FormatName).ToList();
    }

    private void OnRunClicked(object? sender, EventArgs e)
    {
        if (TryParseProgram("programma", out var program))
        {
            RunProgram(program);
        }
    }

    //shows metrics of the program in the editor in the output field
    private void OnMetricsClicked(object? sender, EventArgs e)
    {
        if (!TryParseProgram("programma", out var program))
        {
            return;
        }

        var metrics = _metrics.Calculate(program);
        OutputEditor.Text = $"Aantal commando's: {metrics.CommandCount}{Environment.NewLine}" +
                            $"Maximale nesting: {metrics.MaxNestingLevel}{Environment.NewLine}" +
                            $"Aantal herhalingen: {metrics.RepeatCount}";
        ShowStatus("Metrics berekend.", NeutralColor);
    }

    //opens a txt program file
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
            ShowStatus("Kies een .txt bestand.", ErrorColor);
            return;
        }

        try
        {
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();

            var name = Path.GetFileNameWithoutExtension(file.FileName);
            var program = _programParser.Parse(name, text);

            ProgramEditor.Text = text;
            ShowStatus($"Programma '{program.Name}' is geïmporteerd.", NeutralColor);
        }
        catch (FormatException ex)
        {
            ShowStatus($"Dit bestand is geen geldig programma. {ex.Message}", ErrorColor);
        }
        catch (IOException ex)
        {
            ShowStatus(ex.Message, ErrorColor);
        }
    }

    //saves program in the editor in the chosen format to the documents folder
    private void OnExportClicked(object? sender, EventArgs e)
    {
        if (ExportFormatPicker.SelectedIndex < 0)
        {
            ShowStatus("Kies eerst een exportformaat.", ErrorColor);
            return;
        }

        if (!TryParseProgram("programma", out var program))
        {
            return;
        }

        try
        {
            var exporter = _exporters[ExportFormatPicker.SelectedIndex];
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(folder, program.Name + exporter.FileExtension);

            File.WriteAllText(path, exporter.Export(program));
            ShowStatus($"Geëxporteerd naar: {path}", NeutralColor);
        }
        catch (IOException ex)
        {
            ShowStatus(ex.Message, ErrorColor);
        }
    }

    //puts random program of the chosen level in the program editor
    private void OnLoadRandomClicked(object? sender, EventArgs e)
    {
        if (LevelPicker.SelectedIndex < 0)
        {
            ShowStatus("Kies eerst een niveau.", ErrorColor);
            return;
        }

        var level = Enum.GetValues<ExampleLevel>()[LevelPicker.SelectedIndex];
        LoadIntoEditor(_examples.GetRandom(level));
    }

    //puts the chosen example program in the program editor
    private void OnLoadExampleClicked(object? sender, EventArgs e)
    {
        if (ExamplePicker.SelectedIndex < 0)
        {
            ShowStatus("Kies eerst een voorbeeld.", ErrorColor);
            return;
        }

        LoadIntoEditor(_examplePrograms[ExamplePicker.SelectedIndex]);
    }

    //opens  e xrcise file and shows its grid right away with character on the start cell
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
            var exercise = _exerciseLoader.Load(file.FullPath);
            var grid = exercise.Grid;

            GridEditor.Text = exercise.GridText;
            ExerciseDescriptionLabel.Text = exercise.Description;
            OutputEditor.Text = string.Empty;

            _drawable.Update(grid, [grid.StartPosition], Direction.East);
            GridView.Invalidate();

            ShowStatus($"Oefening '{exercise.Name}' geladen. Schrijf een programma dat Juba naar de x brengt.",
                NeutralColor);
        }
        catch (FormatException ex)
        {
            ShowStatus($"Dit bestand is geen geldige oefening. {ex.Message}", ErrorColor);
        }
        catch (IOException ex)
        {
            ShowStatus(ex.Message, ErrorColor);
        }
    }

    private void OnClearGridClicked(object? sender, EventArgs e)
    {
        GridEditor.Text = string.Empty;
        ExerciseDescriptionLabel.Text = "Laad een oefening, of typ zelf een grid. Laat het grid leeg om vrij te tekenen.";
        _drawable.Clear();
        GridView.Invalidate();
        ShowStatus("Grid gewist, programma's lopen nu zonder grid.", NeutralColor);
    }

    //runs the program on the grid
    private void RunProgram(GridProgram program)
    {
        DomainGrid? grid;
        try
        {
            grid = string.IsNullOrWhiteSpace(GridEditor.Text) ? null : _gridParser.Parse(GridEditor.Text);
        }
        catch (FormatException ex)
        {
            ShowStatus($"Het grid klopt niet: {ex.Message}", ErrorColor);
            return;
        }

        //without grid the character starts at 0,0 in a grid on the start cell; always facing east
        var start = grid?.StartPosition ?? new Position(0, 0);
        var character = new Character(start.X, start.Y, Direction.East);

        try
        {
            var result = grid is null
                ? _executor.Execute(program, character)
                : _executor.Execute(program, grid, character);

            OutputEditor.Text = FormatTrace(result.Trace) + result.FormatEndState();
            _drawable.Update(grid, result.Path, result.FinalDirection);
            ShowOutcome(grid, result);
        }
        catch (ProgramRuntimeException ex)
        {
           
            OutputEditor.Text = FormatTrace(ex.Trace) + $"Gestopt op ({character.X},{character.Y}): {ex.Message}";
            _drawable.Update(grid, ex.Path, character.Direction);
            ShowStatus($"Runtime error: {ex.Message}{NoGridHint(ex, grid)}", ErrorColor);
        }

        GridView.Invalidate();
    }

    //with an exit n the grid the run is a success or a failure
    private void ShowOutcome(DomainGrid? grid, ExecutionResult result)
    {
        if (grid?.Find(CellType.Exit) is not { } exit)
        {
            ShowStatus("Programma uitgevoerd.", NeutralColor);
        }
        else if (_exerciseChecker.ReachedExit(grid, result))
        {
            ShowStatus("Gelukt! Juba heeft de x bereikt.", SuccessColor);
        }
        else
        {
            ShowStatus($"Niet gelukt: Juba eindigt op ({result.FinalX},{result.FinalY}), " +
                       $"de x staat op ({exit.X},{exit.Y}).", FailureColor);
        }
    }

    //without grid the conditions of RepeatUntil are never true (see the assignment), so the loop only stops at the step limit
    private static string NoGridHint(ProgramRuntimeException error, DomainGrid? grid) =>
        error is ExecutionLimitException && grid is null
            ? " RepeatUntil heeft een grid nodig: zonder grid is de voorwaarde nooit waar. Laad eerst een oefening."
            : string.Empty;

    //parses the program editor
    private bool TryParseProgram(string name, out GridProgram program)
    {
        try
        {
            program = _programParser.Parse(name, ProgramEditor.Text ?? string.Empty);
            return true;
        }
        catch (FormatException ex)
        {
            program = new GridProgram(name, []);
            ShowStatus($"Fout in het programma: {ex.Message}", ErrorColor);
            return false;
        }
    }

    private void LoadIntoEditor(GridProgram program)
    {
        ProgramEditor.Text = _textExporter.ToText(program);
        ShowStatus($"Programma '{program.Name}' geladen. Druk op Uitvoeren.", NeutralColor);
    }

    private static string FormatTrace(IReadOnlyList<string> trace) =>
        trace.Count == 0 ? string.Empty : string.Join(Environment.NewLine, trace) + Environment.NewLine + Environment.NewLine;

    private void ShowStatus(string message, Color color)
    {
        StatusLabel.Text = message;
        StatusLabel.TextColor = color;
    }
}
