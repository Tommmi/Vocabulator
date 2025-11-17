using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Domain.Services.AnswerTypes
{
	public class ResponseContextWordAnswer : IResponseContext
	{
		private readonly WordAnswer _answer;
		private readonly bool _isWordInMotherLanguage;

		public ResponseContextWordAnswer(WordAnswer answer, bool isWordInMotherLanguage)
		{
			_answer = answer;
			_isWordInMotherLanguage = isWordInMotherLanguage;
		}

		public List<Vocable> CreateVocables()
		{
			List<Vocable> targetVocabulary = new();
			foreach (var row in _answer.Rows ?? [])
			{
				Console.WriteLine($"{row.Example} - {row.TranslatedExample}");
			}

			foreach (var row in _answer.Rows!.GroupBy(r => r.Translation))
			{
				var newVocable = VocabularyService.CreateVocable(
					leftSentence: $"{row.First().Word}\n[{string.Join(" | ", row.Select(v => v.Context ?? ""))}]",
					rightSentence: $"{row.Key}\n[{string.Join(" | ", row.SelectMany(v => v.AlternativeTranslations ?? []))}]",
					isLeftMotherLanguage: _isWordInMotherLanguage);

				targetVocabulary.Add(newVocable);
			}

			foreach (var row in _answer.Rows!)
			{
				var newVocable = VocabularyService.CreateVocable(
					leftSentence: row.Example!,
					rightSentence: row.TranslatedExample!,
					isLeftMotherLanguage: _isWordInMotherLanguage);

				targetVocabulary.Add(newVocable);
			}

			return targetVocabulary;
		}
	}
}
