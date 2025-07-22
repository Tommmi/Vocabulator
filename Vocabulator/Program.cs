using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator;
using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.EnglishWord;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;
using Vocabulator.OpenAi;

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
            var openAiFactory = new AiEngineFactory(openApiKey: config.ApiKey);
            var processorGermanWord = new Processor4GermanWord(openAiFactory, questionFilePath: options.QuestionGermanWordFilePath);
            var processorEnglishWord = new Processor4EnglishWord(openAiFactory, questionFilePath: options.QuestionEnglishWordFilePath);

            RootObject? answerJson = await processorGermanWord.LoadAnswer(germanWord: "geben");

            if (answerJson != null)
            {
                Console.WriteLine("----- Antwort von OpenAI -----");
                WriteJsonAnswer(answerJson);
            }

            //answerJson = await processorEnglishWord.LoadAnswer(englishWord: "to send");

            //if (answerJson != null)
            //{
            //    Console.WriteLine("----- Antwort von OpenAI -----");
            //    WriteJsonAnswer(answerJson);
            //}
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
    }
}
