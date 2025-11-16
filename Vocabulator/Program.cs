using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Vocabulator.Apps;
using Vocabulator.Common;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services;
using Vocabulator.Domain.Services.QuestionTypes.English4Germans;
using Vocabulator.Domain.Services.QuestionTypes.Germans4English;
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

		private static SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

		private static async Task<T> DoSynchronized<T>(Func<Task<T>> action)
		{
			await _syncLock.WaitAsync();
			try
			{
				return await action();
			}
			finally
			{
				_syncLock.Release();
			}
		}
		private static async Task DoSynchronized(Func<Task> action)
		{
			await _syncLock.WaitAsync();
			try
			{
				await action();
			}
			finally
			{
				_syncLock.Release();
			}
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

			if(options.WordFilePath != null)
			{
				if (options.IsWordInMotherLanguage == null)
				{
					Console.WriteLine($"if 'wordFilePath' is used, you must set parameter 'isWordInMotherLanguage'");
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


            if(options.QuestionFilePath == null)
            {
                throw new ApplicationException("missing config for QuestionFilePath");
            }

            var vocabularyService = CreateVocabularyService(csvFilePath, csvRepo);

            var loadedVocabulary = await LoadVocabularyAsync(vocabularyService);

            if(words.Any())
            {
	            if (!TryGetProcessor(options, config, isWordInMotherLanguage, out var processor))
	            {
		            return;
	            }

				await DoInteruptable(action: async cancellationToken =>
				{
					await ProcessAllNewWords(
						options: options, 
						words: words, 
						cancellationToken: cancellationToken, 
						isWordInMotherLanguage: isWordInMotherLanguage, 
						processor: processor, 
						vocabularyService: vocabularyService, 
						sortService: sortService, 
						loadedVocabulary: loadedVocabulary);
				});

			}
			else
			{
				loadedVocabulary = sortService.Sort(loadedVocabulary);
				await vocabularyService.SaveAsync(loadedVocabulary);
			}
		}

        private static async Task<List<Vocable>> LoadVocabularyAsync(IVocabularyService vocabularyService)
        {
	        var loadedVocabulary = await vocabularyService.TryLoadAsync();
	        if (loadedVocabulary == null)
	        {
		        loadedVocabulary = new List<Vocable>();
	        }

	        return loadedVocabulary;
        }

        private static IVocabularyService CreateVocabularyService(string csvFilePath, CsvRepo csvRepo)
        {
	        IVocabularyService vocabularyService = new VocabularyService(
		        filepath: csvFilePath,
		        columnDescriptions: new List<CsvColumnDescription>
		        {
			        new (HeaderName:"ID", ColumnType:typeof(Guid)),
			        new (HeaderName:"Key", ColumnType:typeof(string)),
			        new (HeaderName:"Translation", ColumnType:typeof(string)),
			        new (HeaderName:"KeyIsMotherLanguage", ColumnType:typeof(bool)),
			        new (HeaderName:"New Words", ColumnType:typeof(string)),
			        new (HeaderName:"Is New", ColumnType:typeof(string)),
		        },
		        csvRepo: csvRepo);
	        return vocabularyService;
        }

        private static async Task ProcessAllNewWords(Options options, List<string> words, CancellationToken cancellationToken,
	        bool isWordInMotherLanguage, IProcessorBase? processor, IVocabularyService vocabularyService,
	        VocabularySortService sortService, List<Vocable>? loadedVocabulary)
        {
	        int cntWords = words.Count;
	        var copiedWords = words.ToList();
	        int bulkSize = 10;

	        for (int i=0; i< cntWords; i += bulkSize)
	        {
		        if (cancellationToken.IsCancellationRequested)
		        {
			        break;
		        }

		        var processingWords = copiedWords.Skip(i).Take(bulkSize);
		        var tasks = new List<Task>();

		        foreach (var word in processingWords)
		        {
			        tasks.Add(HandleWordAsync(
				        options: options, 
				        word: word, 
				        words: words, 
				        isWordInMotherLanguage: isWordInMotherLanguage, 
				        processor: processor, 
				        vocabularyService: vocabularyService, 
				        sortService: sortService, 
				        targetVocabulary: loadedVocabulary));
		        }

		        await Task.WhenAll(tasks);
	        }
        }

        private static async Task HandleWordAsync(
	        Options options, 
	        string word,
	        // mutable
	        List<string> words, 
	        bool isWordInMotherLanguage,
	        IProcessorBase? processor, 
	        IVocabularyService vocabularyService, 
	        VocabularySortService sortService,
	        // mutable
	        List<Vocable> targetVocabulary)
        {
	        await DoSynchronized(action: async () =>
	        {
		        if (IsWordAlreadyUsed(word, targetVocabulary))
		        {
			        RemoveWord(options, words, word);
		        }
	        });

	        var result = await TryAddWordToVocabulary(
		        word: word, 
		        isWordInMotherLanguage: isWordInMotherLanguage, 
		        processor: processor!, 
		        vocabularyService: vocabularyService, 
		        sortService: sortService, 
		        targetVocabulary: targetVocabulary, 
		        shouldSort: false);

	        await DoSynchronized(action: async () =>
	        {
		        if (result.succeeded)
		        {
			        targetVocabulary = result.loadedVocabulary;
			        RemoveWord(options, words, word);
		        }
	        });
        }

        private static bool IsWordAlreadyUsed(string word, List<Vocable> loadedVocabulary)
        {
			word = word.ToLower();
			foreach(var vocable in loadedVocabulary)
			{
				var words = VocabularyService.ExtractWords(vocable.Left.Content);
				if (words.Count == 1 && words.First().ToLower() == word)
				{
					return true;
				}
			}

			return false;
        }

        private static void RemoveWord(Options options, List<string> words, string word)
        {
	        words.Remove(word);
	        SaveWords(options, words);
        }


        private static async Task DoInteruptable(Func<CancellationToken,Task> action)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Run(async () =>
			{
				await action(cancellationToken);
			});

			while (!task.IsCompleted)
			{
				if (Console.KeyAvailable && !cancellationToken.IsCancellationRequested)
				{
					while (Console.KeyAvailable)
					{
						Console.ReadKey(intercept: true);
					}
					Console.WriteLine("stopping ...");
					cancellationTokenSource.Cancel();
				}

				await Task.Delay(100);
			}
		}


		private static bool TryGetWords(Options options, out List<string> words)
        {
			words = new List<string>();
			if (options.Words != null)
			{
				words = options.Words.Split([';','|'],StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
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

				words = fileContent.Split(['\n', '\r'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			return true;
        }

		private static void SaveWords(Options options, List<string> words)
		{
			if (options.WordFilePath != null)
			{
				File.WriteAllText(options.WordFilePath, contents:string.Join('\n',words));
			}
		}


        private static bool TryGetProcessor(
	        Options options,
	        Config config,
			bool isWordInMotherLanguage,
	        out IProcessorBase? processor)
        {
	        var openAiFactory = new AiEngineFactory(openApiKey: config.ApiKey);

			var appDefinitions = new IAppLanguageDefinition[]
			{
				new AppEnglish4Germans(openAiFactory, options.QuestionFilePath!),
				new AppGerman4English(openAiFactory, options.QuestionFilePath!),
			};

			var appDefinition = appDefinitions.FirstOrDefault(a => a.Handles(motherLanguage: options.MyLanguage!, foreignLanguage: options.ForeignLanguage!));

			if(appDefinition == null)
			{
				Console.WriteLine($"mother language {options.MyLanguage} with foreign language {options.ForeignLanguage} is not supported!");
				processor = null;
				return false;
			}

			processor = appDefinition.TryGetProcessor(isWordInMotherLanguage: isWordInMotherLanguage);

	        return processor != null;
        }

        private static bool IsFileLocked(string path)
        {
			if(!File.Exists(path))
			{
				return false;
			}

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



		private static async Task<(bool succeeded, List<Vocable> loadedVocabulary)> TryAddWordToVocabulary(
	        string word, 
            bool isWordInMotherLanguage,
	        IProcessorBase processor, 
	        IVocabularyService vocabularyService,
	        VocabularySortService sortService, 
	        List<Vocable> targetVocabulary,
	        bool shouldSort)
        {
	        var answer = await processor.LoadAnswer(word: word);
	        if (answer != null)
	        {
				return await DoSynchronized(action: async () =>
				{
					foreach (var row in answer.Rows ?? [])
					{
						Console.WriteLine($"{row.Example} - {row.TranslatedExample}");
					}

					foreach (var row in answer.Rows!.GroupBy(r => r.Translation))
					{
						var newVocable = vocabularyService.CreateVocable(
							leftSentence: $"{row.First().Word}\n[{string.Join(" | ", row.Select(v => v.Context ?? ""))}]",
							rightSentence: $"{row.Key}\n[{string.Join(" | ", row.SelectMany(v => v.AlternativeTranslations ?? []))}]",
							isLeftMotherLanguage: isWordInMotherLanguage);

						targetVocabulary.Add(newVocable);
					}

					foreach (var row in answer.Rows!)
					{
						var newVocable = vocabularyService.CreateVocable(
							leftSentence: row.Example!,
							rightSentence: row.TranslatedExample!,
							isLeftMotherLanguage: isWordInMotherLanguage);

						targetVocabulary.Add(newVocable);
					}

					if (shouldSort)
					{
						targetVocabulary = sortService.Sort(targetVocabulary);
					}

					await vocabularyService.SaveAsync(targetVocabulary);
					return (succeeded: true, loadedVocabulary: targetVocabulary);
				});
	        }
			else
	        {
		        Console.WriteLine($"can't find answer {word}");
		        return (succeeded: false, targetVocabulary);
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
