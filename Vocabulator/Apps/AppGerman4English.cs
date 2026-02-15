using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.Processors.English4Germans;
using Vocabulator.Domain.Services.Processors.Germans4English;

namespace Vocabulator.Apps;

internal class AppGerman4English : IAppLanguageDefinition
{
	private readonly Processor4EnglishGerman4English _processorEnglishGerman4English;
	private readonly Processor4GermanEnglish4English _processorGermanEnglish4English;
	private readonly Processor4GermanEnglishTranslation _processorGermanEnglishTranslation4English;
	private readonly Processor4EnglishGermanTranslation _processorEnglishGermanTranslation4English;

	public AppGerman4English(IAiEngineFactory openAiFactory, string questionFilePath,
		string questionTranslationFilePath)
	{
		_processorEnglishGerman4English = new Processor4EnglishGerman4English(openAiFactory, questionFilePath: questionFilePath);
		_processorGermanEnglish4English = new Processor4GermanEnglish4English(openAiFactory, questionFilePath: questionFilePath);
		_processorGermanEnglishTranslation4English = new Processor4GermanEnglishTranslation(openAiFactory, questionFilePath: questionTranslationFilePath, isGermanMotherlanguage: false);
		_processorEnglishGermanTranslation4English = new Processor4EnglishGermanTranslation(openAiFactory, questionFilePath: questionTranslationFilePath, isEnglishMotherlanguage:true);
	}

	public bool Handles(string motherLanguage, string foreignLanguage, bool isGrammarSession, bool isTranslationSession)
	{
		return !isGrammarSession && motherLanguage == "English" && foreignLanguage == "German";
	}

	public IProcessorBase? TryGetProcessor(bool isWordInMotherLanguage, bool isGrammarSession, bool isTranslationSession)
	{
		if(isGrammarSession)
		{
			if(isTranslationSession)
			{
				throw new ArgumentException();
			}
			return null;
		}

		if(isTranslationSession)
		{
			return isWordInMotherLanguage ? _processorEnglishGermanTranslation4English : _processorGermanEnglishTranslation4English;
		}

		return isWordInMotherLanguage ? _processorGermanEnglish4English : _processorEnglishGerman4English;
	}
}