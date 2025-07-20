using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;

namespace Vocabulator.Domain.Services.QuestionTypes.EnglishWord
{
    public class Processor4EnglishWord : GermanOrEnglishWordBase<Processor4EnglishWord.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string englishWord) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",englishWord)
                    })
            {

            }
        }

        public Processor4EnglishWord(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4EnglishWord))
        {
        }

        public async Task<RootObject?> LoadAnswer(string englishWord)
        {
            QuestionType question = new QuestionType(_aiProcessor.QuestionTemplate, englishWord: englishWord);
            return await base.DoRequest(question);
        }
    }
}
