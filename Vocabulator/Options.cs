using CommandLine;

namespace Vocabulator;

internal class Options
{
	[Option('w', "words", Required = false, HelpText = "words to be translated, separated by ; or |", Default = null)]
	public string? Words { get; set; }
	[Option('m', "isWordInMotherLanguage", Required = false, HelpText = "true, if word is in mother language")]
	public bool? IsWordInMotherLanguage { get; set; }
	[Option('f', "wordFilePath", Required = false, HelpText = "file path to file of words")]
	public string? WordFilePath { get; set; }
	[Option('f', "csvFilePath", Required = true, HelpText = "file path to csv file. May still not exist.")]
	public string? CsvFilePath { get; set; }
	[Option('l', "myLanguage", Required = true, HelpText = "what's my language ('english','german','brazilian')")]
	public string? MyLanguage { get; set; }
	[Option('f', "foreignLanguage", Required = true, HelpText = "what's the foreign language ('english','german','brazilian')")]
	public string? ForeignLanguage { get; set; }
	[Option('g', "isGrammar", Required = false, HelpText = "true, if is grammar task")]
	public bool? IsGrammarTask { get; set; }

}