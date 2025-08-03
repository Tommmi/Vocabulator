using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Vocabulator.Common;

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

            using var client = new HttpClient();

            // TODO: entfernen
            //Console.WriteLine("###################################################");
            //Console.WriteLine("OpenAI API Request");
            //Console.WriteLine("###################################################");
            //Console.WriteLine("Request:");
            //Console.WriteLine(question.Text);

            string? responseString = await TryCallOpenAi(client, _openApiKey, inputText);

            if (responseString != null)
            {
                var resultText = GetResultText(responseString);

                if (resultText != null)
                {
                    //Console.WriteLine("Response:");
                    //Console.WriteLine(resultText);
                    return DeserializeJsonAnswer(resultText);
                }
            }

            return null;
        }

        private void ValidateQuestion(Question question)
        {
            if (question.Template.TemplateName != _questionTemplate.TemplateName)
            {
                throw new ArgumentException($"template {_questionTemplate.TemplateName} expected, but got {question.Template.TemplateName}");
            }
        }

        static string ExtractJson(string input)
        {
            int endIndex = input.LastIndexOf('}');
            if (endIndex == -1)
                throw new FormatException("Keine schließende Klammer gefunden.");

            int startIndex = input.IndexOf('{');
            if (startIndex == -1 || startIndex >= endIndex)
                throw new FormatException("Keine öffnende Klammer gefunden oder sie liegt hinter der letzten schließenden.");

            return input.Substring(startIndex, endIndex - startIndex + 1);
        }


        private static TResponse? DeserializeJsonAnswer(string resultText)
        {
            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)resultText;
            }

            resultText = ExtractJson(resultText);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var answerJson = JsonSerializer.Deserialize<TResponse>(resultText, jsonOptions);
            return answerJson;
        }



        private static string? GetResultText(string? responseString)
        {
            using var jsonDoc = JsonDocument.Parse(responseString);
            var resultText = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            return resultText;
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
                model = "gpt-4o",
                temperature = 0.2,
                top_p = 0.9,
                presence_penalty = 0,
                frequency_penalty = 0,
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

            string responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler ({response.StatusCode}):");
                Console.WriteLine(responseString);
                return null;
            }

            return responseString;
        }

    }
}
