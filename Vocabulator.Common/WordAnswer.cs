using System.Text.Json.Serialization;

namespace Vocabulator.Common
{
    public class WordAnswer
    {
	    [JsonPropertyName("rows")]
        public Row[]? Rows { get; set; }
    }
}
