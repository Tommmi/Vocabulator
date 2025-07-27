using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord;

public class ContextEntry
{
    [JsonPropertyName("a-translation")]

    public string Translation { get; set; }
    public Example[] Examples { get; set; }
}