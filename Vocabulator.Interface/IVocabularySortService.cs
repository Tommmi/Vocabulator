using Vocabulator.Common;

namespace Vocabulator.Domain.Interface
{
	public interface IVocabularySortService
	{
		List<Vocable> Sort(List<Vocable> vocables);

		(int line, Word word)? TryFindUntranslatedWord(List<Vocable> vocables);

	}
}
