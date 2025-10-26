using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Vocabulator.Common;
using Vocabulator.OpenAi.Common;

namespace Vocabulator.OpenAi
{
    public class OpenAiEngine<TResponse> : IAiEngine<TResponse> where TResponse: class
    {
        private readonly QuestionTemplate _questionTemplate;
        private readonly string _openApiKey;

        public OpenAiEngine(QuestionTemplate questionTemplate, string openApiKey)
        {
            _questionTemplate = questionTemplate;
            _openApiKey = openApiKey;
        }

        public async Task<TResponse?> DoRequest(Question question)
        {
            ValidateQuestion(question);

            var inputText = question.Text;

            string? responseString;


			using (var client = new HttpClient())
            {
	            client.Timeout = TimeSpan.FromMinutes(3);
	            responseString = await TryCallOpenAi(client, _openApiKey, inputText);
            }
               

            var json = ExtractLastJson(responseString);

            OpenAiResponse? openAiResponse = DeserializeOpenAiResponse(json);
            
            json = ExtractLastJson(openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content);

			return DeserializeJsonAnswer(json);
        }

        private OpenAiResponse? DeserializeOpenAiResponse(string? resultText)
        {
	        if(resultText == null)
	        {
                return null;
	        }

            return JsonSerializer.Deserialize<OpenAiResponse>(resultText);
		}

        private void ValidateQuestion(Question question)
        {
            if (question.Template.TemplateName != _questionTemplate.TemplateName)
            {
                throw new ArgumentException($"template {_questionTemplate.TemplateName} expected, but got {question.Template.TemplateName}");
            }
        }


        private static TResponse? DeserializeJsonAnswer(string? json)
        {
            if(json == null)
            {
                return null;
            }

            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)json;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var answerJson = JsonSerializer.Deserialize<TResponse>(json, jsonOptions);
            return answerJson;
        }

        public static string? ExtractLastJson(string? source)
        {
	        if (string.IsNullOrWhiteSpace(source))
		        return null;

	        if (!source.Contains('{') && !source.Contains('['))
		        return null;

	        var pattern = @"(\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*(?(open)(?!))\})"  
	                      + @"|(\[(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!))\])"; 

	        var matches = Regex.Matches(source, pattern, RegexOptions.Singleline);

	        if (matches.Count == 0)
		        return null;

	        string lastJsonCandidate = matches[^1].Value.Trim();

	        try
	        {
		        using var doc = JsonDocument.Parse(lastJsonCandidate);
		        return doc.RootElement.GetRawText();
	        }
	        catch (JsonException)
	        {
		        for (int i = matches.Count - 2; i >= 0; i--)
		        {
			        try
			        {
				        using var doc = JsonDocument.Parse(matches[i].Value.Trim());
				        return doc.RootElement.GetRawText();
			        }
			        // ReSharper disable once EmptyGeneralCatchClause
			        catch
			        {  }
		        }

		        return null;
	        }
        }


		private static async Task<string?> TryCallOpenAi(HttpClient client, string apiKey, string? inputText)
        {
            var content = CreateHttpStringContent(client, apiKey, inputText);

            return await TryCallOpenAi(client, content);
        }

        private static StringContent CreateHttpStringContent(HttpClient client, string apiKey, string? inputText)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-5-mini",
                stream = false,
                messages = new[]
                {
                    new { role = "user", content = inputText }
                }

            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            return content;
        }

        private static async Task<string?> TryCallOpenAi(HttpClient client, StringContent content)
        {
            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Anfrage: {ex.Message}");
                return null;
            }

            try
            {
	            string responseString = await response.Content.ReadAsStringAsync();

	            if (!response.IsSuccessStatusCode)
	            {
		            Console.WriteLine($"Fehler ({response.StatusCode}):");
		            Console.WriteLine(responseString);
		            return null;
	            }

	            return responseString;
            }
            catch (Exception ex)
            {
	            Console.WriteLine($"{ex.Message}");
	            return null;
            }

		}

	}
}
