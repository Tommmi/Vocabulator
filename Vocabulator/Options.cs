using CommandLine;

namespace OpenAiConsoleApp;

internal class Options
{
    [Option('a', "questionEnglishWordExt1", Required = false, HelpText = "filepath to questionEnglishWordExt1", Default = ".\\questionEnglishWordExt1.txt")]
    public string QuestionEnglishWordExt1FilePath { get; set; }
    [Option('b', "questionEnglishWordExt2", Required = false, HelpText = "filepath to questionEnglishWordExt2", Default = ".\\questionEnglishWordExt2.txt")]
    public string QuestionEnglishWordExt2FilePath { get; set; }
    [Option('i', "questionGermanWordExt1", Required = false, HelpText = "filepath to questionGermanWordExt1", Default = ".\\questionGermanWordExt1.txt")]
    public string QuestionGermanWordExt1FilePath { get; set; }
    [Option('j', "questionGermanWordExt2", Required = false, HelpText = "filepath to questionGermanWordExt2", Default = ".\\questionGermanWordExt2.txt")]
    public string QuestionGermanWordExt2FilePath { get; set; }
}