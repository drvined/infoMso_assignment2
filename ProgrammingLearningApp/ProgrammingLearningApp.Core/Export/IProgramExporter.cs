using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Export;

//one export format for a program (Strategy pattern): the UI only knows this interface,
//so a new format is added by writing one more class and adding it to the list of exporters
public interface IProgramExporter
{
    //the name shown to the user, for example "Tekst (.txt)"
    string FormatName { get; }

    //the extension of the file that is saved, for example ".txt"
    string FileExtension { get; }

    string Export(GridProgram program);
}
