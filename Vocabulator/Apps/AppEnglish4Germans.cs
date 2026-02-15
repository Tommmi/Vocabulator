using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.Processors.English4Germans;

namespace Vocabulator.Apps
{
	internal class AppEnglish4Germans : IAppLanguageDefinition
	{
		private readonly Processor4EnglishGerman4Germans _processorEnglishGerman4Germans;
		private readonly Processor4EnglishGermanTranslation _processorEnglishGermanTranslation4Germans;
		private readonly Processor4GermanEnglishTranslation _processorGermanEnglishTranslation4Germans;
		private readonly Processor4GermanEnglish4Germans _processorGermanEnglish4Germans;
		private readonly Processor4EnglishGermanGrammar4Germans _processorEnglishGermanGrammar4Germans;
		private readonly Processor4GermanEnglishGrammar4Germans _processorGermanEnglishGrammar4Germans;

		public AppEnglish4Germans(
			IAiEngineFactory openAiFactory, 
			string questionFilePath, 
			string questionGrammarFilePath,
			string questionTranslationFilePath
			)
		{
			_processorEnglishGerman4Germans = new Processor4EnglishGerman4Germans(openAiFactory, questionFilePath: questionFilePath);
			_processorGermanEnglish4Germans = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: questionFilePath);
			_processorEnglishGermanGrammar4Germans = new Processor4EnglishGermanGrammar4Germans(openAiFactory, questionFilePath: questionGrammarFilePath);
			_processorGermanEnglishGrammar4Germans = new Processor4GermanEnglishGrammar4Germans(openAiFactory, questionFilePath: questionGrammarFilePath);
			_processorEnglishGermanTranslation4Germans = new Processor4EnglishGermanTranslation(openAiFactory, questionFilePath: questionTranslationFilePath, isEnglishMotherlanguage: false);
			_processorGermanEnglishTranslation4Germans = new Processor4GermanEnglishTranslation(openAiFactory, questionFilePath: questionTranslationFilePath, isGermanMotherlanguage: true);
		}
		public bool Handles(string motherLanguage, string foreignLanguage, bool isGrammarSession, bool isTranslationSession)
		{
			return motherLanguage == "German" && foreignLanguage == "English";
		}

		public IProcessorBase TryGetProcessor(bool isWordInMotherLanguage, bool isGrammarSession, bool isTranslationSession)
		{
			if(isGrammarSession)
			{
				if (isTranslationSession)
				{
					throw new ArgumentException();
				}
				return isWordInMotherLanguage ? _processorGermanEnglishGrammar4Germans : _processorEnglishGermanGrammar4Germans;
			}
			else
			{
				if (isTranslationSession)
				{
					return isWordInMotherLanguage ? _processorGermanEnglishTranslation4Germans : _processorEnglishGermanTranslation4Germans;
				}
				else
				{
					return isWordInMotherLanguage ? _processorGermanEnglish4Germans : _processorEnglishGerman4Germans;
				}
			}
		}
	}
}
