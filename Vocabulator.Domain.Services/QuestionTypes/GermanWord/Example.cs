using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord;

public class Example
{
    public string German { get; set; }
    public string English { get; set; }

    [JsonPropertyName("used_translation")]
    public string UsedTranslation { get; set; }
    
}