using Vocabulator.Domain.Interface;

namespace Vocabulator
{
	internal interface IAppLanguageDefinition
	{
		bool Handles(string motherLanguage, string foreignLanguage, bool isGrammarSession);
		IProcessorBase? TryGetProcessor(bool isWordInMotherLanguage, bool isGrammarSession);

	}
}
