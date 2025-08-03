using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step2
{
    public class Processor4EnglishWord2 : Processor4GermanWord2
    {

        public Processor4EnglishWord2(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(aiEngineFactory, questionFilePath)
        {
        }
    }
}
