using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1
{
    public class Translation
    {
        [JsonPropertyName("translation")]
        public string TranslationText { get; set; }

        public Example Example { get; set; }
    }
}
