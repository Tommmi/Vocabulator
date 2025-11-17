using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.QuestionTypes.English4Germans;

public class Processor4GermanEnglishGrammar4Germans : ProcessorBase<GrammarAnswer, Processor4GermanEnglishGrammar4Germans.QuestionType>
{
	public class QuestionType : Question
	{
		public QuestionType(QuestionTemplate template, string word)
			: base(
				template,
				parameters:
				[
					new("WORD", word),
					new("deutsche", "deutsche")
				])
		{

		}
	}

	public Processor4GermanEnglishGrammar4Germans(IAiEngineFactory aiEngineFactory, string questionFilePath)
		: base(
			aiEngineFactory: aiEngineFactory,
			questionFilePath: questionFilePath,
			className: nameof(Processor4GermanEnglishGrammar4Germans))
	{
	}

	public override async Task<IResponseContext?> LoadAnswer(string word)
	{
		var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
		var answer = await _aiProcessor.DoRequest(question);
		if (answer == null)
		{
			return null;
		}
		return new ResponseContextGrammarAnswer(answer, isLeftInMotherLanguage:true);
	}
}