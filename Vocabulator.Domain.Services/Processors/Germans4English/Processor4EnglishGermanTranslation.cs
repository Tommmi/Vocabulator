using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.English4Germans;

public class Processor4GermanEnglishTranslation : ProcessorBase<TranslationAnswer, Processor4GermanEnglishTranslation.QuestionType>
{
	private readonly bool _isGermanMotherlanguage;

	public class QuestionType : Question
	{
		public QuestionType(QuestionTemplate template, string word)
			: base(
				template,
				parameters:
				[
					new("WORD", word),
					new("deutsche", "deutsche"),
					new("englische", "englische")
				])
		{

		}
	}

	public Processor4GermanEnglishTranslation(IAiEngineFactory aiEngineFactory, string questionFilePath, bool isGermanMotherlanguage)
		: base(
			aiEngineFactory: aiEngineFactory,
			questionFilePath: questionFilePath,
			className: nameof(Processor4GermanEnglishTranslation))
	{
		_isGermanMotherlanguage = isGermanMotherlanguage;
	}

	public override async Task<IResponseContext?> LoadAnswer(string word)
	{
		var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
		var answer = await _aiProcessor.DoRequest(question);
		if (answer == null)
		{
			return null;
		}
		return new ResponseContextTranslationAnswer(word,answer,isLeftMotherLanguage: _isGermanMotherlanguage);
	}
}