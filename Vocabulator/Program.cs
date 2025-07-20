using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator;

namespace OpenAiConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            Config config = ReadConfig();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    Process(options, config).Wait();
                });
        }

        private static async Task Process(Options options, Config config)
        {
            var inputText = await TryReadQuestion(options);
            string apiKey = config.ApiKey;

            using var client = new HttpClient();

            var responseString = await TryCallOpenAi(client, apiKey, inputText);

            if(responseString != null) 
            {
                var resultText = GetResultText(responseString);

                var answerJson = DeserializeJsonAnswer(ref resultText);


                Console.WriteLine("----- Antwort von OpenAI -----");
                Console.WriteLine(resultText);

                WriteJsonAnswer(answerJson);
            }
        }

        private static void WriteJsonAnswer(RootObject? answerJson)
        {
            Console.WriteLine("----- Antwort von OpenAI parsed -----");

            // Ausgabe zum Test
            Console.WriteLine($"Wort: {answerJson.Word}");
            foreach (var context in answerJson.Contexts)
            {
                Console.WriteLine($"\nKontext: {context.Context}");
                Console.WriteLine("Übersetzungen: " + string.Join(", ", context.Translations));
                foreach (var example in context.Examples)
                {
                    Console.WriteLine($"  - {example.German} => {example.English}");
                }
            }
        }

        private static RootObject? DeserializeJsonAnswer([AllowNull] ref string resultText)
        {
            resultText = ExtractJson(resultText);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var answerJson = JsonSerializer.Deserialize<RootObject>(resultText, jsonOptions);
            return answerJson;
        }

        private static async Task<string?> TryCallOpenAi(HttpClient client, string apiKey, string? inputText)
        {
            var content = CreateHttpStringContent(client, apiKey, inputText);

            return await TryCallOpenAi(client, content);
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

        private static StringContent CreateHttpStringContent(HttpClient client, string apiKey, string? inputText)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "user", content = inputText }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            return content;
        }

        private static Config ReadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            string apiKey = config["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.Write("Bitte API-Key eingeben: ");
                    apiKey = Console.ReadLine();
                }
            }

            return new Config(apiKey: apiKey);
        }

        private static async Task<string?> TryReadQuestion(Options options)
        {
            string questionFilePath = options.QuestionFilePath;

            if (!File.Exists(questionFilePath))
            {
                Console.WriteLine($"Datei nicht gefunden: {questionFilePath}");
                return null;
            }

            // Inhalt lesen
            string? inputText = await File.ReadAllTextAsync(questionFilePath);
            return inputText;
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


    }
}
