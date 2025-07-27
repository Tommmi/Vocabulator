using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord
{
    public abstract class GermanOrEnglishWordBase<TQuestionType> : ProcessorBase<RootObject, TQuestionType> where TQuestionType: Question
    {
        protected GermanOrEnglishWordBase(IAiEngineFactory aiEngineFactory, string questionFilePath, string className) 
            : base(aiEngineFactory, questionFilePath, className)
        {
        }

        protected async Task<RootObject?> DoRequest(TQuestionType question)
        {
            var response = await _aiProcessor.DoRequest(question);


            return response;
        }

    }
}
