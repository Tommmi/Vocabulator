using Vocabulator.Common;
using WordAnswer2 = Vocabulator.Domain.Services.QuestionTypes.GermanWord2;


namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord5
{
    public class Processor4GermanWord5 : ProcessorBase<WordAnswer, Processor4GermanWord5.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string answer1) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(ANSWER1)PARAM",answer1),
                    })
            {

            }
        }

        public Processor4GermanWord5(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord5))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(WordAnswer2.WordAnswer answer1)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, answer1:Serialize(answer1));
            return await _aiProcessor.DoRequest(question);
        }
    }
}
