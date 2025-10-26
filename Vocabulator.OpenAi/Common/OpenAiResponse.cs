using System.Text.Json.Serialization;

namespace Vocabulator.OpenAi.Common
{
	public class OpenAiResponse
	{
		public OpenAiResponse(List<OpenAiChoice> choices)
		{
			Choices = choices;
		}
		public OpenAiResponse()
		{
		}


		[JsonPropertyName("choices")]
		public List<OpenAiChoice>? Choices { get; set; }
	}
}
