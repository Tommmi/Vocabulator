namespace Vocabulator.Common;

public interface IAiQuestionProcessor<TResponse, TQuestionType> where TQuestionType : Question
{
    Task<TResponse?> DoRequest(TQuestionType question);
}