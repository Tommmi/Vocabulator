using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord1;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord2;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord3;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord4;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord5;
using Vocabulator.Domain.Services.QuestionTypes.GermanWordExt1;
using Vocabulator.Domain.Services.QuestionTypes.GermanWordExt2;
using AnswerWord2 = Vocabulator.Domain.Services.QuestionTypes.GermanWord2;
using AnswerWord3 = Vocabulator.Domain.Services.QuestionTypes.GermanWord3;
using AnswerWord5 = Vocabulator.Domain.Services.QuestionTypes.GermanWord5;

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
            var processorGermanWordExt1 = new Processor4GermanWordExt1(openAiFactory, questionFilePath: options.QuestionGermanWordExt1FilePath);
            var processorGermanWordExt2 = new Processor4GermanWordExt2(openAiFactory, questionFilePath: options.QuestionGermanWordExt2FilePath);

            string word = "halten";
            var answer1 = await processorGermanWordExt1.LoadAnswer(word: word);
            if (answer1 != null)
            {
                var answer2 = await processorGermanWordExt2.LoadAnswer(word: word, answer1:answer1);
                if (answer2 != null)
                {

                }

            }

            //var processorGermanWord1 = new Processor4GermanWord1(openAiFactory, questionFilePath: options.QuestionGermanWord1FilePath);
            //var processorGermanWord2 = new Processor4GermanWord2(openAiFactory, questionFilePath: options.QuestionGermanWord2FilePath);
            //var processorGermanWord3 = new Processor4GermanWord3(openAiFactory, questionFilePath: options.QuestionGermanWord3FilePath);
            //var processorGermanWord4 = new Processor4GermanWord4(openAiFactory, questionFilePath: options.QuestionGermanWord4FilePath);
            //var processorGermanWord5 = new Processor4GermanWord5(openAiFactory, questionFilePath: options.QuestionGermanWord5FilePath);

            //string word = "sagen";
            //string? answer1 = await processorGermanWord1.LoadAnswer(germanWord: word);

            //if (answer1 != null)
            //{
            //    Console.WriteLine("----- processorGermanWord Step 1 -----");

            //    var answer2 = await processorGermanWord2.LoadAnswer(germanWord: word, answer1:answer1);
            //    if (answer2 != null)
            //    {
            //        Console.WriteLine("----- processorGermanWord Step 2 -----");

            //        var answer3 = await processorGermanWord3.LoadAnswer(answer1: answer2);
            //        if (answer3 != null)
            //        {
            //            Console.WriteLine("----- processorGermanWord Step 3 -----");

            //            var answer4 = await processorGermanWord4.LoadAnswer(germanWord:word,answer1: answer3);
            //            if (answer4 != null)
            //            {
            //                Console.WriteLine("----- processorGermanWord Step 4 -----");

            //                var answer5 = await processorGermanWord5.LoadAnswer(answer1: answer4);
            //                if (answer5 != null)
            //                {
            //                    Console.WriteLine("----- processorGermanWord Step 5 -----");
            //                    WriteJsonAnswer(answer5);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private static void WriteJsonAnswer(AnswerWord5.WordAnswer? answerJson)
        {
            Console.WriteLine("----- Antwort von OpenAI parsed -----");

            // Ausgabe zum Test
            Console.WriteLine($"Wort: {answerJson.Word}");
            foreach (var translation in answerJson.Translations)
            {
                Console.WriteLine($"context:{translation.Context}");
                Console.WriteLine("Übersetzung: " + string.Join(", ", translation.Translation));
                foreach(var example in  translation.Examples)
                {
                    Console.WriteLine("Example: " + example);
                }
            }
        }
        private static void WriteJsonAnswer(AnswerWord2.WordAnswer? answerJson)
        {
            Console.WriteLine("----- Antwort von OpenAI parsed -----");

            // Ausgabe zum Test
            Console.WriteLine($"Wort: {answerJson.Word}");
            foreach (var translation in answerJson.Translations)
            {
                Console.WriteLine($"context:{translation.Context}");
                Console.WriteLine("Übersetzung: " + string.Join(", ", translation.Translation));
            }
        }

        private static void WriteJsonAnswer(AnswerWord3.WordAnswer? answerJson)
        {
            Console.WriteLine("----- Antwort von OpenAI parsed -----");

            // Ausgabe zum Test
            Console.WriteLine($"Wort: {answerJson.Word}");
            foreach (var translation in answerJson.Translations)
            {
                Console.WriteLine($"context:{translation.Context}");
                Console.WriteLine("Übersetzung: " + string.Join(", ", translation.Translation));
                foreach (var german in translation.German)
                {
                    Console.WriteLine($"Rückübersetzung: {german}");
                }
            }
        }


        private static void WriteJsonAnswer(RootObject? answerJson)
        {
            Console.WriteLine("----- Antwort von OpenAI parsed -----");

            // Ausgabe zum Test
            Console.WriteLine($"Wort: {answerJson.Word}");
            foreach (var context in answerJson.Translations)
            {
                Console.WriteLine("Übersetzung: " + string.Join(", ", context.Translation));
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
