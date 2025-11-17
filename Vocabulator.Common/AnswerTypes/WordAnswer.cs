using System.Text.Json.Serialization;

namespace Vocabulator.Common.AnswerTypes
{
    public class WordAnswer
    {
	    [JsonPropertyName("rows")]
        public Row[]? Rows { get; set; }
    }
}
