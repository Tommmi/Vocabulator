using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;

namespace Vocabulator.Domain.Services.QuestionTypes.EnglishWord
{
    public class Processor4ExtendGermanATranslation : ProcessorBase<RootObject, Processor4ExtendGermanATranslation.QuestionType> 
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, RootObject aTranslations) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",aTranslations.Word),
                        new KeyValuePair<string, string>("PARAM(ATRANSLATIONS)PARAM",Serialize(aTranslations)),
                    })
            {

            }
        }

        public Processor4ExtendGermanATranslation(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4EnglishWord))
        {
        }

        public async Task<RootObject?> LoadAnswer(RootObject aTranslations)
        {
            QuestionType question = new QuestionType(_aiProcessor.QuestionTemplate, aTranslations: aTranslations);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
