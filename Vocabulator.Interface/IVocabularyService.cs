using Vocabulator.Common;

namespace Vocabulator.Domain.Interface
{
	public interface IVocabularyService
	{
		Task<List<Vocable>?> TryLoadAsync();
		Task SaveAsync(List<Vocable> vocables);
		Vocable CreateVocable(string leftSentence, string rightSentence, bool isLeftMotherLanguage);
	}
}
