using Vocabulator.Common;

namespace Vocabulator.Domain.Interface;

public interface IProcessorBase
{
	Task<IResponseContext?> LoadAnswer(string word);
}


public interface IResponseContext
{
	List<Vocable> CreateVocables();
}