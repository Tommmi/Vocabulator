using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator.Common;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services;
using Vocabulator.Domain.Services.QuestionTypes.English4Germans;
using Vocabulator.Infrastructure;

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
            bool isWordInMotherLanguage = options.IsWordInMotherLanguage ?? false;
            string? word = options.Word;
            string csvFilePath = options.CsvFilePath!;
            bool automatic = options.Automatic ?? false;

            var csvRepo = new CsvRepo();
            var sortService = new VocabularySortService();

			var openAiFactory = new AiEngineFactory(openApiKey: config.ApiKey);
            if(options.QuestionFilePath == null)
            {
                throw new ApplicationException("missing config for QuestionFilePath");
            }

            IVocabularyService vocabularyService = new VocabularyService(
	            filepath: csvFilePath,
	            columnDescriptions: new List<CsvColumnDescription>
	            {
		            new (HeaderName:"ID", ColumnType:typeof(Guid)),
		            new (HeaderName:"Key", ColumnType:typeof(string)),
		            new (HeaderName:"Translation", ColumnType:typeof(string)),
		            new (HeaderName:"KeyIsMotherLanguage", ColumnType:typeof(bool)),
				},
	            csvRepo: csvRepo);

            var loadedVocabulary = await vocabularyService.TryLoadAsync();
            if (loadedVocabulary == null)
            {
	            loadedVocabulary = new List<Vocable>();
            }

            var processorEnglishGerman4Germans = new Processor4EnglishGerman4Germans(openAiFactory, questionFilePath: options.QuestionFilePath);
	        var processorGermanEnglish4Germans = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: options.QuestionFilePath);


            if(word != null)
            {
	            IProcessorBase processor = isWordInMotherLanguage ? processorGermanEnglish4Germans : processorEnglishGerman4Germans;

	            loadedVocabulary = await AddWord(word, isWordInMotherLanguage,processor, vocabularyService, sortService, loadedVocabulary);
            }

            if(automatic)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                var task = Task.Run(async () =>
                {
                    while(!cancellationToken.IsCancellationRequested)
                    {
	                    var unknownWord = sortService.TryFindUntranslatedWord(loadedVocabulary);

	                    if (unknownWord.HasValue)
	                    {
		                    string untranslatedWord = unknownWord.Value.word.Token;

		                    loadedVocabulary = await AddWord(
			                    untranslatedWord,
			                    isWordInMotherLanguage: false,
			                    processorEnglishGerman4Germans,
			                    vocabularyService,
			                    sortService,
			                    loadedVocabulary);
	                    }
                    }
				});

                while(!task.IsCompleted)
                {
                    if(Console.KeyAvailable && !cancellationTokenSource.IsCancellationRequested)
                    {
                        while(Console.KeyAvailable)
                        {
	                        _ = Console.ReadKey(intercept: false);
						}

                        cancellationTokenSource.Cancel();
                        Console.WriteLine("Key pressed. Stopping process ...");
					}
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

				Console.WriteLine("... process stopped");


			}
		}

        private static async Task<List<Vocable>> AddWord(
	        string word, 
            bool isWordInMotherLanguage,
	        IProcessorBase processor, 
	        IVocabularyService vocabularyService,
	        VocabularySortService sortService, 
	        List<Vocable> loadedVocabulary)
        {
	        var answer = await processor.LoadAnswer(word: word);
	        if (answer != null)
	        {
		        foreach (var row in answer.Rows ?? [])
		        {
			        Console.WriteLine($"{row.Example} - {row.TranslatedExample}");
		        }

		        foreach (var row in answer.Rows!)
		        {
			        var newVocable = vocabularyService.CreateVocable(
				        leftSentence: $"{row.Word}\n[{row.Context!}]",
				        rightSentence: $"{row.Translation!}\n[{row.AlternativeTranslation}]",
				        isLeftMotherLanguage: isWordInMotherLanguage);

			        loadedVocabulary.Add(newVocable);

			        newVocable = vocabularyService.CreateVocable(
				        leftSentence: row.Example!,
				        rightSentence: row.TranslatedExample!,
				        isLeftMotherLanguage: isWordInMotherLanguage);

			        loadedVocabulary.Add(newVocable);
		        }

				loadedVocabulary = sortService.Sort(loadedVocabulary);
		        await vocabularyService.SaveAsync(loadedVocabulary);
	        }
	        else
	        {
		        Console.WriteLine("can't find answer");
	        }

	        return loadedVocabulary;
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
