using System.Text.Json.Serialization;

namespace Vocabulator.Common;

public class GrammarSentence
{
	[JsonPropertyName("sentence")]
	public string? Sentence { get; set; }
	[JsonPropertyName("translation")]
	public string? Translation { get; set; }
}