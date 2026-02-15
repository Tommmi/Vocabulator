using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.German4Brazilians;

public class Processor4GermanBrazilianGrammar4Brazilians : ProcessorBase<GrammarAnswer, Processor4GermanBrazilianGrammar4Brazilians.QuestionType>
{
	public class QuestionType : Question
	{
		public QuestionType(QuestionTemplate template, string word)
			: base(
				template,
				parameters:
				[
					new("WORD", word),
					new("deutsche", "deutsche"),
					new("englische", "brasilianische"),
				])
		{

		}
	}

	public Processor4GermanBrazilianGrammar4Brazilians(IAiEngineFactory aiEngineFactory, string questionFilePath)
		: base(
			aiEngineFactory: aiEngineFactory,
			questionFilePath: questionFilePath,
			className: nameof(Processor4GermanBrazilianGrammar4Brazilians))
	{
	}

	public override async Task<IResponseContext?> LoadAnswer(string word)
	{
		var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
		var answer = await _aiProcessor.DoRequest(question);
		if(answer == null)
		{
			return null;
		}
		return new ResponseContextGrammarAnswer(answer, isLeftInMotherLanguage:false);
	}
}