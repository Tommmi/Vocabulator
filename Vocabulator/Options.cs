using CommandLine;

namespace Vocabulator;

internal class Options
{
	[Option('q', "question", Required = false, HelpText = "filepath to question", Default = ".\\question.txt")]
	public string? QuestionFilePath { get; set; }
	[Option('w', "word", Required = false, HelpText = "word to be translated", Default = null)]
	public string? Word { get; set; }
	[Option('m', "isWordInMotherLanguage", Required = true, HelpText = "true, if word is in mother language")]
	public bool? IsWordInMotherLanguage { get; set; }
	[Option('f', "csvFilePath", Required = true, HelpText = "file path to csv file. May still not exist.")]
	public string? CsvFilePath { get; set; }
	[Option('a', "automatic", Required = false, HelpText = "true, if program looks for words automatically", Default = false)]
	public bool? Automatic { get; set; }


}