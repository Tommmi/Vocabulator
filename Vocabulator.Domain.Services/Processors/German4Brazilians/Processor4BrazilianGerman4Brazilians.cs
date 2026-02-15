using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.German4Brazilians
{
    public class Processor4BrazilianGerman4Brazilians : ProcessorBase<WordAnswer, Processor4BrazilianGerman4Brazilians.QuestionType>
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
	                    new("Muttersprache", "brasilianisch")
					])
            {

            }
        }

        public Processor4BrazilianGerman4Brazilians(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4BrazilianGerman4Brazilians))
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
            return new ResponseContextWordAnswer(answer, isWordInMotherLanguage:true);
        }
	}
}
