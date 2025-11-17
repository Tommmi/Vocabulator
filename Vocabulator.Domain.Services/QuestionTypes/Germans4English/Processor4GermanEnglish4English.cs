using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.QuestionTypes.Germans4English
{
    public class Processor4GermanEnglish4English : ProcessorBase<WordAnswer, Processor4GermanEnglish4English.QuestionType>
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
	                    new("englische", "englische"),
	                    new("Muttersprache", "englisch")
					])
            {

            }
        }

        public Processor4GermanEnglish4English(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanEnglish4English))
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

	        return new ResponseContextWordAnswer(answer, isWordInMotherLanguage: false);
        }
    }
}
