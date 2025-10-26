using System.Text.Json.Serialization;

namespace Vocabulator.OpenAi.Common;

public class OpenAiMessage
{
	public OpenAiMessage(string content)
	{
		Content = content;
	}
	public OpenAiMessage()
	{
	}

	[JsonPropertyName("content")]
	public string? Content { get; set; }

}