using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Export;

//strategy pattern export format 
public interface IProgramExporter
{
    string FormatName { get; }

    string FileExtension { get; }

    string Export(GridProgram program);
}
