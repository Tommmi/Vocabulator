using Vocabulator.Common;


using Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWordExt2
{
    public class Processor4GermanWordExt2 : ProcessorBase<WordAnswer, Processor4GermanWordExt2.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string word, string answer1) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",word),
                        new KeyValuePair<string, string>("PARAM(ANSWER1)PARAM",answer1)
                    })
            {

            }
        }

        public Processor4GermanWordExt2(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWordExt2))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string word, WordAnswer answer1)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word, answer1:Serialize(answer1));
            return await _aiProcessor.DoRequest(question);
        }
    }
}
