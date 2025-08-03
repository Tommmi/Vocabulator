using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1
{
    public class Processor4GermanWord1 : ProcessorBase<WordAnswer, Processor4GermanWord1.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string word) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",word),
                    })
            {

            }
        }

        public Processor4GermanWord1(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord1))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string word)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
