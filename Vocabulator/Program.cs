using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator;
using Vocabulator.Domain.Services.QuestionTypes.EnglishGerman4Germans.step1;
using Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1;
using Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step2;

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
            var processorGermanWordExt1 = new Processor4GermanWord1(openAiFactory, questionFilePath: options.QuestionGermanWordExt1FilePath);
            var processorGermanWordExt2 = new Processor4GermanWord2(openAiFactory, questionFilePath: options.QuestionGermanWordExt2FilePath);
            var processorEnglishWordExt1 = new Processor4EnglishWord1(openAiFactory, questionFilePath: options.QuestionEnglishWordExt1FilePath);
            var processorEnglishWordExt2 = new Processor4EnglishWord2(openAiFactory, questionFilePath: options.QuestionEnglishWordExt2FilePath);

            string word = "think";
            var answer1 = await processorEnglishWordExt1.LoadAnswer(word: word);
            if (answer1 != null)
            {
                var answer2 = await processorEnglishWordExt2.LoadAnswer(word: word, answer1:answer1);
                if (answer2 != null)
                {
                    var result = answer2.GetTrainingData(word);
                    foreach(var item in result)
                    {
                        Console.WriteLine($"{item.Left} -> {item.Right}");
                    }
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
