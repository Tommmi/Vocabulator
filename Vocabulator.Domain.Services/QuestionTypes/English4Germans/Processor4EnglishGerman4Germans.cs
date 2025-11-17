using Vocabulator.Common;
using Vocabulator.Common.AnswerTypes;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.AnswerTypes;

namespace Vocabulator.Domain.Services.QuestionTypes.English4Germans
{
    public class Processor4EnglishGerman4Germans : ProcessorBase<WordAnswer, Processor4EnglishGerman4Germans.QuestionType>
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
	                    new("Muttersprache", "deutsch")
					])
            {

            }
        }

        public Processor4EnglishGerman4Germans(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanEnglish4Germans))
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
            return new ResponseContextWordAnswer(answer, isWordInMotherLanguage:false);
        }
    }
}
