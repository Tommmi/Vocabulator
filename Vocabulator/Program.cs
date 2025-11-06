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
                    if(!ValidateOptions(options))
                    {
                        return;
                    }

                    Process(options, config).Wait();
                });
        }

        private static bool ValidateOptions(Options options)
        {
	        if(options.Words != null)
	        {
                if(options.IsWordInMotherLanguage == null)
                {
                    Console.WriteLine($"if 'word' is used, you must set parameter 'isWordInMotherLanguage'");
                    return false;
				}
				if(options.WordFilePath != null)
				{
					Console.WriteLine($"if 'word' is used, you may not use parameter 'wordFilePath'");
					return false;
				}
			}

            return true;
        }

        private static async Task Process(Options options, Config config)
        {
            bool isWordInMotherLanguage = options.IsWordInMotherLanguage ?? false;

			if(!TryGetWords(options, out var words))
			{
				return;
			}

            string csvFilePath = options.CsvFilePath!;

            var csvRepo = new CsvRepo();
            var sortService = new VocabularySortService();

            if (IsFileLocked(csvFilePath))
            {
                Console.WriteLine($"file {csvFilePath} ist geöffnet. Bitte schließen!");
                return;
            }


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
		            new (HeaderName:"New Words", ColumnType:typeof(string)),
				},
	            csvRepo: csvRepo);

            var loadedVocabulary = await vocabularyService.TryLoadAsync();
            if (loadedVocabulary == null)
            {
	            loadedVocabulary = new List<Vocable>();
            }

            if(words.Any())
            {
	            if (!TryGetProcessor(options, openAiFactory, isWordInMotherLanguage, out var processor))
	            {
		            return;
	            }

				foreach(var word in words)
				{
					loadedVocabulary = await AddWord(word, isWordInMotherLanguage, processor!, vocabularyService, sortService, loadedVocabulary);
				}
			}
			else
			{
				loadedVocabulary = sortService.Sort(loadedVocabulary);
				await vocabularyService.SaveAsync(loadedVocabulary);
			}
		}

        private static bool TryGetWords(Options options, out string[] words)
        {
			words = [];
			if (options.Words != null)
			{
				words = options.Words.Split([';','|'],StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			}
			else if(options.WordFilePath != null)
			{
				if(!File.Exists(options.WordFilePath))
				{
					Console.WriteLine($"file doese not exist {options.WordFilePath}");
					return false;
				}

				string fileContent;

				if (IsFileLocked(options.WordFilePath))
				{
					Console.WriteLine($"file {options.WordFilePath} ist geöffnet. Bitte schließen!");
					return false;
				}

				fileContent = File.ReadAllText(options.WordFilePath);

				words = fileContent.Split(['\n', '\r'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			}

			return true;
        }

        private static bool TryGetProcessor(
	        Options options, 
	        AiEngineFactory openAiFactory, 
	        bool isWordInMotherLanguage,
	        out IProcessorBase? processor)
        {
	        var processorEnglishGerman4Germans = new Processor4EnglishGerman4Germans(openAiFactory, questionFilePath: options.QuestionFilePath!);
	        var processorGermanEnglish4Germans = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: options.QuestionFilePath!);

	        switch (options.MyLanguage)
	        {
		        case "German":
			        switch(options.ForeignLanguage)
			        {
				        case "English":
					        processor = isWordInMotherLanguage ? processorGermanEnglish4Germans : processorEnglishGerman4Germans;
					        break;
				        case "Brazilian":
					        throw new ArgumentException();
				        default:
					        Console.WriteLine($"foreign language {options.ForeignLanguage} is not supported!");
					        processor = null;
					        return false;
			        }
			        break;
		        case "English":
			        throw new ArgumentException();
		        case "Brazilian":
			        throw new ArgumentException();
		        default:
			        Console.WriteLine($"mother language {options.MyLanguage} is not supported!");
			        processor = null;
			        return false;
	        }

	        return true;
        }

        private static bool IsFileLocked(string path)
        {
	        try
	        {
		        using var stream = new FileStream(
			        path,
			        FileMode.Open,
			        FileAccess.ReadWrite,
			        FileShare.None // exklusiver Zugriff
		        );
		        return false; // kein Fehler → Datei ist NICHT gesperrt
	        }
	        catch (IOException)
	        {
		        return true; // IOException → Datei wird gerade verwendet
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
