namespace ProgrammingLearningApp.Parsing;

//splits text into lines, whatever line endings it uses: \r\n (Windows files), \n (Linux and Mac files)
//or only \r (the text box of the Windows app), used by both the program parser and the grid parser
internal static class TextLines
{
    public static string[] Split(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
}
