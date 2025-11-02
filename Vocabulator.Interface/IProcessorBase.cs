using Vocabulator.Common;

namespace Vocabulator.Domain.Interface;

public interface IProcessorBase
{
	Task<WordAnswer?> LoadAnswer(string word);
}