using System.Text.Json.Serialization;

namespace Vocabulator.Domain.Services.QuestionTypes
{
	public class Row
	{
		[JsonPropertyName("word")]
		public string? Word { get; set; }
		[JsonPropertyName("translation")]
		public string? Translation { get; set; }
		[JsonPropertyName("context")]
		public string? Context { get; set; }
		[JsonPropertyName("example")]
		public string? Example { get; set; }

		[JsonPropertyName("translated-example")]
		public string? TranslatedExample { get; set; }
		[JsonPropertyName("explanation")]
		public string? Explanation { get; set; }
		[JsonPropertyName("alternative-translation")]
		public string? AlternativeTranslation { get; set; }
	}
}
