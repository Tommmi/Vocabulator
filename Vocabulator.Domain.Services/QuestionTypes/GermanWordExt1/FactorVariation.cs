using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1
{
    public class FactorVariation
    {
        public string Variation { get; set; }
        public Translation[] Translations { get; set; }
    }
}
