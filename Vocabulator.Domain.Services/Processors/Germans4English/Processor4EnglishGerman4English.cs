using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.Processors.Germans4English
{
    public class Processor4EnglishGerman4English : ProcessorBase<WordAnswer, Processor4EnglishGerman4English.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string word) 
                : base(
                    template, 
                    parameters:
                    [
	                    new("WORD", word),
	                    new("deutsche", "englische"),
	                    new("englische", "deutsche"),
	                    new("Muttersprache", "englisch")
					])
            {

            }
        }

        public Processor4EnglishGerman4English(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4EnglishGerman4English))
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
