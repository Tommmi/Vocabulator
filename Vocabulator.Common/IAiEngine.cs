namespace Vocabulator.Common
{
    public interface IAiEngine<TResponse>
    {
        Task<TResponse?> DoRequest(Question question);
    }
}
