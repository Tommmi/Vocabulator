using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.Processors.German4Brazilians;

namespace Vocabulator.Apps
{
	internal class AppGerman4Brazilians : IAppLanguageDefinition
	{
		private readonly Processor4BrazilianGerman4Brazilians _processor4BrazilianGerman4Brazilians;
		private readonly Processor4BrazilianGermanGrammar4Brazilians _processor4BrazilianGermanGrammar4Brazilians;
		private readonly Processor4GermanBrazilian4Brazilians _processor4GermanBrazilian4Brazilians;
		private readonly Processor4GermanBrazilianGrammar4Brazilians _processor4GermanBrazilianGrammar4Brazilians;

		public AppGerman4Brazilians(IAiEngineFactory openAiFactory, string questionFilePath, string questionGrammarFilePath)
		{
			_processor4BrazilianGerman4Brazilians = new Processor4BrazilianGerman4Brazilians(openAiFactory, questionFilePath: questionFilePath);
			_processor4GermanBrazilian4Brazilians = new Processor4GermanBrazilian4Brazilians(openAiFactory, questionFilePath: questionFilePath);
			_processor4BrazilianGermanGrammar4Brazilians = new Processor4BrazilianGermanGrammar4Brazilians(openAiFactory, questionFilePath: questionGrammarFilePath);
			_processor4GermanBrazilianGrammar4Brazilians = new Processor4GermanBrazilianGrammar4Brazilians(openAiFactory, questionFilePath: questionGrammarFilePath);
		}

		public bool Handles(string motherLanguage, string foreignLanguage, bool isGrammarSession, bool isTranslationSession)
		{
			return !isTranslationSession && motherLanguage == "Brazilian" && foreignLanguage == "German";
		}

		public IProcessorBase TryGetProcessor(bool isWordInMotherLanguage, bool isGrammarSession, bool isTranslationSession)
		{
			if(isGrammarSession)
			{
				return isWordInMotherLanguage ? _processor4BrazilianGermanGrammar4Brazilians : _processor4GermanBrazilianGrammar4Brazilians;
			}
			else
			{
				return isWordInMotherLanguage ? _processor4BrazilianGerman4Brazilians : _processor4GermanBrazilian4Brazilians;
			}
		}
	}
}
