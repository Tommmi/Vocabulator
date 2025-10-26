using System.Text.Json.Serialization;

namespace Vocabulator.OpenAi.Common;

public class OpenAiChoice
{
	public OpenAiChoice(OpenAiMessage message)
	{
		Message = message;
	}

	public OpenAiChoice()
	{
	}

	[JsonPropertyName("message")]
	public OpenAiMessage? Message { get; set; }
}