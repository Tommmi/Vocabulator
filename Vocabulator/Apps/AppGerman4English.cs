using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.QuestionTypes.Germans4English;

namespace Vocabulator.Apps;

internal class AppGerman4English : IAppLanguageDefinition
{
	private readonly Processor4EnglishGerman4English _processorEnglishGerman4English;
	private readonly Processor4GermanEnglish4English _processorGermanEnglish4English;

	public AppGerman4English(IAiEngineFactory openAiFactory, string questionFilePath)
	{
		_processorEnglishGerman4English = new Processor4EnglishGerman4English(openAiFactory, questionFilePath: questionFilePath);
		_processorGermanEnglish4English = new Processor4GermanEnglish4English(openAiFactory, questionFilePath: questionFilePath);
	}
	public bool Handles(string motherLanguage, string foreignLanguage)
	{
		return motherLanguage == "English" && foreignLanguage == "German";
	}

	public IProcessorBase? TryGetProcessor(bool isWordInMotherLanguage)
	{
		return isWordInMotherLanguage ? _processorGermanEnglish4English : _processorEnglishGerman4English;
	}
}