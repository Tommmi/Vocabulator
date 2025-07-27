using Vocabulator.Common;


namespace Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1
{
    public class Processor4GermanWordExt1 : ProcessorBase<WordAnswer, Processor4GermanWordExt1.QuestionType>
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

        public Processor4GermanWordExt1(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWordExt1))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string word)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
