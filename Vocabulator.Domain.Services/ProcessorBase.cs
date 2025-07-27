using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ude;
using Vocabulator.Common;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;

namespace Vocabulator.Domain.Services;

public abstract class ProcessorBase<TResponse,TQuestionType> 
    where TResponse : class 
    where TQuestionType: Question
{
    private readonly IAiEngineFactory _aiEngineFactory;
    protected readonly AiQuestionProcessor<TResponse, TQuestionType> _aiProcessor;

    protected ProcessorBase(IAiEngineFactory aiEngineFactory, string questionFilePath, string className)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _aiEngineFactory = aiEngineFactory;
        _aiProcessor = new AiQuestionProcessor<TResponse, TQuestionType>(
            templateName: className,
            template: GetTemplate(questionFilePath).Result!,
            aiEngineCreator: CreateAiEngineFunc);
    }
    private IAiEngine<TResponse> CreateAiEngineFunc(QuestionTemplate questionTemplate)
    {
        return _aiEngineFactory.Create<TResponse>(questionTemplate);
    }
    private async Task<string?> GetTemplate(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Datei nicht gefunden: {filePath}");
            return null;
        }

        var detector = new CharsetDetector();
        
        await using (var fs = File.OpenRead(filePath))
        {
            detector.Feed(fs);
            detector.DataEnd();
        }

        Console.WriteLine($"Detected charset: {detector.Charset}");

        if (detector.Charset != null)
        {
            var encoding = Encoding.GetEncoding(detector.Charset);
            using var reader = new StreamReader(filePath, encoding);
            string inputText = await reader.ReadToEndAsync();
            return inputText;
        }
        throw new ApplicationException("tag9862654");
    }

    protected static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}