using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1
{
    public class Factor
    {
        [JsonPropertyName("factor")]
        public string FactorName { get; set; }

        
        [JsonPropertyName("factor-variations")]
        public FactorVariation[] FactorVariations { get; set; }
    }
}
