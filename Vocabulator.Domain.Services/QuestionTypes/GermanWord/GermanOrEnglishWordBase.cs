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

            if(response != null)
            {
                List<int> indexesToBeDeleted = new List<int>();

                for (int iContext = 0; iContext < response.Contexts.Length ; iContext++)
                {
                    if(indexesToBeDeleted.Contains(iContext))
                    {
                        continue;
                    }
                    var context = response.Contexts[iContext];
                    var translations = context.Translations.ToList();
                    for (int iContext2 = iContext+1; iContext2 < response.Contexts.Length; iContext2++)
                    {
                        var context2 = response.Contexts[iContext2];
                        var translations2 = context2.Translations.ToList();
                        var translationsCommon = translations.Intersect(translations2);
                        if(translationsCommon.Any())
                        {
                            context.Translations = translationsCommon.ToArray();
                            translations = translationsCommon.ToList();
                            context.Context += "|" + context2.Context;
                            context.Examples = context.Examples.Concat(context2.Examples).ToArray();
                            indexesToBeDeleted.Add(iContext2);
                        }
                    }
                }
            }

            return response;
        }

    }
}
