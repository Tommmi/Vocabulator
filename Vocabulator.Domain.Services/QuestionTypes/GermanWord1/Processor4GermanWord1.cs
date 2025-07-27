using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord1
{
    public class Processor4GermanWord1 : ProcessorBase<string, Processor4GermanWord1.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string germanWord) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",germanWord)
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

        public async Task<string?> LoadAnswer(string germanWord)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, germanWord: germanWord);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
