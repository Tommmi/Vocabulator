using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1;

namespace Vocabulator.Domain.Services.QuestionTypes.EnglishGerman4Germans.step1
{
    public class Processor4EnglishWord1 : Processor4GermanWord1
    {
        public Processor4EnglishWord1(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(aiEngineFactory, questionFilePath)
        {

        }
    }
}
