using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord2;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord3
{
    public class Processor4GermanWord3 : ProcessorBase<WordAnswer, Processor4GermanWord3.QuestionType>
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

        public Processor4GermanWord3(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord3))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(GermanWord2.WordAnswer answer1)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, answer1: Serialize(answer1));
            return await _aiProcessor.DoRequest(question);
        }
    }
}
