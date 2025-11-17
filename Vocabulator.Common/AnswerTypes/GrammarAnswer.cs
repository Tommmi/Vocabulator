using System.Text.Json.Serialization;

namespace Vocabulator.Common.AnswerTypes;

public class GrammarAnswer
{
	[JsonPropertyName("examples")]
	public GrammarSentence[]? GrammarSentences { get; set; }
}