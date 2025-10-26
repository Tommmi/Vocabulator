using CommandLine;

namespace Vocabulator;

internal class Options
{
    [Option('q', "question", Required = false, HelpText = "filepath to question", Default = ".\\question.txt")]
    public string? QuestionFilePath { get; set; }
}