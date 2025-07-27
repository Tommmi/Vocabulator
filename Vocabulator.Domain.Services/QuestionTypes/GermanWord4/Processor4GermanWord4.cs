using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord2;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord4
{
    public class Processor4GermanWord4 : ProcessorBase<WordAnswer, Processor4GermanWord4.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string germanWord, string answer1) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",germanWord),
                        new KeyValuePair<string, string>("PARAM(ANSWER1)PARAM",answer1),
                    })
            {

            }
        }

        public Processor4GermanWord4(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord4))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string germanWord, GermanWord3.WordAnswer answer1)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, germanWord: germanWord, answer1:Serialize(answer1));
            return await _aiProcessor.DoRequest(question);
        }
    }
}
