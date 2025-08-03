using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1
{
    public class Translation
    {
        [JsonPropertyName("translation")]
        public string TranslationText { get; set; }

        public Example Example { get; set; }
    }
}
