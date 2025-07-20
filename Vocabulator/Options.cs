using CommandLine;

namespace OpenAiConsoleApp;

internal class Options
{
    [Option('a', "questionGermanWordFilePath", Required = false, HelpText = "questionGermanWordFilePath", Default = ".\\questionGermanWordFilePath.txt")]
    public string QuestionGermanWordFilePath { get; set; }
    [Option('b', "questionEnglishWordFilePath", Required = false, HelpText = "questionEnglishWordFilePath", Default = ".\\questionEnglishWordFilePath.txt")]
    public string QuestionEnglishWordFilePath { get; set; }
}