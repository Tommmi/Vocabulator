using CommandLine;

namespace OpenAiConsoleApp;

internal class Options
{
    [Option('a', "questionGermanWordFilePath", Required = false, HelpText = "questionGermanWordFilePath", Default = ".\\questionGermanWordFilePath.txt")]
    public string QuestionGermanWordFilePath { get; set; }
    [Option('b', "questionEnglishWordFilePath", Required = false, HelpText = "questionEnglishWordFilePath", Default = ".\\questionEnglishWordFilePath.txt")]
    public string QuestionEnglishWordFilePath { get; set; }
    [Option('c', "questionGermanATranslationsFilePath", Required = false, HelpText = "questionGermanATranslations", Default = ".\\questionGermanATranslations.txt")]
    public string QuestionGermanATranslations { get; set; }
    [Option('d', "questionGermanWord1", Required = false, HelpText = "filepath to questionGermanWord1", Default = ".\\questionGermanWord1.txt")]
    public string QuestionGermanWord1FilePath { get; set; }
    [Option('e', "questionGermanWord2", Required = false, HelpText = "filepath to questionGermanWord2", Default = ".\\questionGermanWord2.txt")]
    public string QuestionGermanWord2FilePath { get; set; }
    [Option('f', "questionGermanWord3", Required = false, HelpText = "filepath to questionGermanWord3", Default = ".\\questionGermanWord3.txt")]
    public string QuestionGermanWord3FilePath { get; set; }
    [Option('g', "questionGermanWord4", Required = false, HelpText = "filepath to questionGermanWord4", Default = ".\\questionGermanWord4.txt")]
    public string QuestionGermanWord4FilePath { get; set; }
    [Option('h', "questionGermanWord5", Required = false, HelpText = "filepath to questionGermanWord5", Default = ".\\questionGermanWord5.txt")]
    public string QuestionGermanWord5FilePath { get; set; }
    [Option('i', "questionGermanWordExt1", Required = false, HelpText = "filepath to questionGermanWordExt1", Default = ".\\questionGermanWordExt1.txt")]
    public string QuestionGermanWordExt1FilePath { get; set; }
    [Option('j', "questionGermanWordExt2", Required = false, HelpText = "filepath to questionGermanWordExt2", Default = ".\\questionGermanWordExt2.txt")]
    public string QuestionGermanWordExt2FilePath { get; set; }
}