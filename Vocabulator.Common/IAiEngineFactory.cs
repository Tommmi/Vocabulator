
namespace Vocabulator.Common
{
    public interface IAiEngineFactory
    {
        IAiEngine<TResponse> Create<TResponse>(QuestionTemplate questionTemplate) where TResponse : class;
    }
}
