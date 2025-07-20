using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord
{
    public class Processor4GermanWord : ProcessorBase<RootObject>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string germanWord) 
                : base(
                    template, 
                    parameters:new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("PARAM(WORD)PARAM",germanWord)
                    })
            {

            }
        }

        public Processor4GermanWord(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4GermanWord))
        {
        }

        public async Task<RootObject?> LoadAnswer(string germanWord)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, germanWord: germanWord);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
