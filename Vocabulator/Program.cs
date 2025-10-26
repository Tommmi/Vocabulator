using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans;

namespace Vocabulator
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
            if(options.QuestionFilePath == null)
            {
                throw new ApplicationException("missing config for QuestionFilePath");
            }
            var processor = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: options.QuestionFilePath);

            string word = "denken";
            var answer = await processor.LoadAnswer(word: word);
            if (answer != null)
            {
                foreach (var row in answer.Rows ?? [])
                {
                    Console.WriteLine($"{row.Example} - {row.TranslatedExample}");
                }
            }
            else
            {
                Console.WriteLine("can't find answer");
            }
        }

        private static Config ReadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            string? apiKey = config["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.Write("Bitte API-Key eingeben: ");
                    apiKey = Console.ReadLine();
                }
            }

            return new Config(apiKey: apiKey!);
        }
    }
}
