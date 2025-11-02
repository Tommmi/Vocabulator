using Vocabulator.Common;

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

        public override async Task<WordAnswer?> LoadAnswer(string word)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
