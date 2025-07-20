using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;
using Vocabulator.OpenAi;

namespace Vocabulator
{
    public class AiEngineFactory : IAiEngineFactory
    {
        private readonly string _openApiKey;

        public AiEngineFactory(string openApiKey)
        {
            _openApiKey = openApiKey;
        }
        public IAiEngine<TResponse> Create<TResponse>(QuestionTemplate questionTemplate) where TResponse : class
        {
            return new OpenAiEngine<TResponse>(questionTemplate, openApiKey: _openApiKey);
        }
    }
}
