using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulator.Common
{
    public interface IAiEngineFactory
    {
        IAiEngine<TResponse> Create<TResponse>(QuestionTemplate questionTemplate) where TResponse : class;
    }
}
