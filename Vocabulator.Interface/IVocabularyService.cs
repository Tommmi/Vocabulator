using Vocabulator.Common;

namespace Vocabulator.Domain.Interface
{
	public interface IVocabularyService
	{
		Task<List<Vocable>?> TryLoadAsync();
		Task SaveAsync(List<Vocable> vocables);
	}
}
