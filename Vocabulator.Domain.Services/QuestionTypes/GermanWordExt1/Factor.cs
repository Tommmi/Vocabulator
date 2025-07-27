using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1
{
    public class Factor
    {
        [JsonPropertyName("factor")]
        public string FactorName { get; set; }

        
        [JsonPropertyName("factor-variations")]
        public FactorVariation[] FactorVariations { get; set; }
    }
}
