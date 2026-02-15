using System.Linq;
using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Domain.Services.AnswerTypes;

public class ResponseContextTranslationAnswer : IResponseContext
{
	private readonly string _word;
	private readonly TranslationAnswer _answer;
	private readonly bool _isLeftMotherLanguage;

	public ResponseContextTranslationAnswer(string word, TranslationAnswer answer, bool isLeftMotherLanguage)
	{
		_word = word;
		_answer = answer;
		_isLeftMotherLanguage = isLeftMotherLanguage;
	}

	public List<Vocable> CreateVocables()
	{
		foreach (var t in _answer.TranslationOptions)
		{
			Console.WriteLine($"{_word} - {t}");
		}

		return new List<Vocable>
		{
			VocabularyService.CreateVocable(
				leftSentence:_word,
				rightSentence:$"{_answer.TranslationOptions?.FirstOrDefault()??""}\n[{string.Join("|\n",_answer.TranslationOptions.Skip(1))}]",
				isLeftMotherLanguage:_isLeftMotherLanguage)
		};
	}
}