using CommandLine;

namespace OpenAiConsoleApp;

internal class Options
{
    [Option('f', "file", Required = false, HelpText = "path to question file", Default = ".\\question.txt")]
    public string QuestionFilePath { get; set; }
}