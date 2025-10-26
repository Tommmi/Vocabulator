using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans
{
    public class Processor4GermanEnglish4Germans : ProcessorBase<WordAnswer, Processor4GermanEnglish4Germans.QuestionType>
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
	                    new("englische", "englische")
					])
            {

            }
        }

        public Processor4GermanEnglish4Germans(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanEnglish4Germans))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string word)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
