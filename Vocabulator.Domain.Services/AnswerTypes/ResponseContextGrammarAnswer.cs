using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Domain.Services.AnswerTypes;

public class ResponseContextGrammarAnswer : IResponseContext
{
	private readonly GrammarAnswer _answer;
	private readonly bool _isLeftInMotherLanguage;

	public ResponseContextGrammarAnswer(GrammarAnswer answer, bool isLeftInMotherLanguage )
	{
		_answer = answer;
		_isLeftInMotherLanguage = isLeftInMotherLanguage;
	}

	public List<Vocable> CreateVocables()
	{
		return _answer
			.GrammarSentences?
			.Where(s=>s.Sentence != null && s.Translation != null)
			.Select(s => VocabularyService.CreateVocable(
				leftSentence: s.Sentence!, 
				rightSentence: s.Translation!, 
				isLeftMotherLanguage: _isLeftInMotherLanguage))
			.ToList()
			??new List<Vocable>();
	}
}