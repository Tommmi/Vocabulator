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

        string? inputText = await File.ReadAllTextAsync(filePath);
        return inputText;
    }
}