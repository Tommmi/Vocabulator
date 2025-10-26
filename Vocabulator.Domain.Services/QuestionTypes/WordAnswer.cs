using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes
{
    public class WordAnswer
    {
	    [JsonPropertyName("rows")]
        public Row[]? Rows { get; set; }
    }
}
