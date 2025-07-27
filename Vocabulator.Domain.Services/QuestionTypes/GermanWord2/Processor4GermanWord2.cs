using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord2
{
    public class Processor4GermanWord2 : ProcessorBase<WordAnswer, Processor4GermanWord2.QuestionType>
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

        public Processor4GermanWord2(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord2))
        {
        }

        public async Task<WordAnswer?> LoadAnswer(string germanWord, string answer1)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, germanWord: germanWord, answer1:answer1);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
