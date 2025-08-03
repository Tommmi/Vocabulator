using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulator.Common
{
    public interface IResult
    {
        List<ITrainingDataItem> GetTrainingData(string word);
    }
}
