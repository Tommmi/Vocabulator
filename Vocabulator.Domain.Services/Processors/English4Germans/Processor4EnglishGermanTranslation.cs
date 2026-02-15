using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.English4Germans;

public class Processor4EnglishGermanTranslation : ProcessorBase<TranslationAnswer, Processor4EnglishGermanTranslation.QuestionType>
{
	private readonly bool _isEnglishMotherlanguage;

	public class QuestionType : Question
	{
		public QuestionType(QuestionTemplate template, string word)
			: base(
				template,
				parameters:
				[
					new("WORD", word),
					new("deutsche", "englische"),
					new("englische", "deutsche")
				])
		{

		}
	}

	public Processor4EnglishGermanTranslation(IAiEngineFactory aiEngineFactory, string questionFilePath, bool isEnglishMotherlanguage)
		: base(
			aiEngineFactory: aiEngineFactory,
			questionFilePath: questionFilePath,
			className: nameof(Processor4EnglishGermanTranslation))
	{
		_isEnglishMotherlanguage = isEnglishMotherlanguage;
	}

	public override async Task<IResponseContext?> LoadAnswer(string word)
	{
		var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
		var answer = await _aiProcessor.DoRequest(question);
		if (answer == null)
		{
			return null;
		}
		return new ResponseContextTranslationAnswer(word,answer,isLeftMotherLanguage: _isEnglishMotherlanguage);
	}
}