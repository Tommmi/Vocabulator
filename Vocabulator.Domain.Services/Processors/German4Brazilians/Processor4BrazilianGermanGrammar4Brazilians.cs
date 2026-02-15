using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.German4Brazilians;

public class Processor4BrazilianGermanGrammar4Brazilians : ProcessorBase<GrammarAnswer, Processor4BrazilianGermanGrammar4Brazilians.QuestionType>
{
	public class QuestionType : Question
	{
		public QuestionType(QuestionTemplate template, string word)
			: base(
				template,
				parameters:
				[
					new("WORD", word),
					new("deutsche", "brasilianische"),
					new("englische", "deutsche"),
				])
		{

		}
	}

	public Processor4BrazilianGermanGrammar4Brazilians(IAiEngineFactory aiEngineFactory, string questionFilePath)
		: base(
			aiEngineFactory: aiEngineFactory,
			questionFilePath: questionFilePath,
			className: nameof(Processor4BrazilianGermanGrammar4Brazilians))
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