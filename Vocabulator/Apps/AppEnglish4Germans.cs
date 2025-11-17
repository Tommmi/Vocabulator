using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.QuestionTypes.English4Germans;

namespace Vocabulator.Apps
{
	internal class AppEnglish4Germans : IAppLanguageDefinition
	{
		private readonly Processor4EnglishGerman4Germans _processorEnglishGerman4Germans;
		private readonly Processor4GermanEnglish4Germans _processorGermanEnglish4Germans;
		private readonly Processor4EnglishGermanGrammar4Germans _processorEnglishGermanGrammar4Germans;
		private readonly Processor4GermanEnglishGrammar4Germans _processorGermanEnglishGrammar4Germans;

		public AppEnglish4Germans(IAiEngineFactory openAiFactory, string questionFilePath, string questionGrammarFilePath)
		{
			_processorEnglishGerman4Germans = new Processor4EnglishGerman4Germans(openAiFactory, questionFilePath: questionFilePath);
			_processorGermanEnglish4Germans = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: questionFilePath);
			_processorEnglishGermanGrammar4Germans = new Processor4EnglishGermanGrammar4Germans(openAiFactory, questionFilePath: questionGrammarFilePath);
			_processorGermanEnglishGrammar4Germans = new Processor4GermanEnglishGrammar4Germans(openAiFactory, questionFilePath: questionGrammarFilePath);
		}
		public bool Handles(string motherLanguage, string foreignLanguage, bool isGrammarSession)
		{
			return motherLanguage == "German" && foreignLanguage == "English";
		}

		public IProcessorBase TryGetProcessor(bool isWordInMotherLanguage, bool isGrammarSession)
		{
			if(isGrammarSession)
			{
				return isWordInMotherLanguage ? _processorGermanEnglishGrammar4Germans : _processorEnglishGermanGrammar4Germans;
			}
			else
			{
				return isWordInMotherLanguage ? _processorGermanEnglish4Germans : _processorEnglishGerman4Germans;
			}
		}
	}
}
