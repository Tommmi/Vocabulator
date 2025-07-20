using System.Text.RegularExpressions;
using Vocabulator.Common;

namespace Vocabulator.Domain.Services
{
    public class AiQuestionProcessor<TResponse, TQuestionType> 
        : IAiQuestionProcessor<TResponse, TQuestionType> where TQuestionType : Question
    {
        private readonly IAiEngine<TResponse> _aiEngine;
        public QuestionTemplate QuestionTemplate { get; }

        public AiQuestionProcessor(string templateName, string @template, Func<QuestionTemplate,IAiEngine<TResponse>> aiEngineCreator)
        {
            QuestionTemplate = new QuestionTemplate(
                templateName: templateName, 
                template: @template, 
                parameterNames: ParseParameters(@template));
            _aiEngine = aiEngineCreator(QuestionTemplate);
        }

        private List<string> ParseParameters(string @template)
        {
            string startMarker = "PARAM(";
            string endMarker = ")PARAM";

            List<string> parameterNames = new();

            int index = 0;
            while ((index = @template.IndexOf(startMarker, index, StringComparison.Ordinal)) != -1)
            {
                int start = index;
                int pos = index + startMarker.Length;

                while (pos < @template.Length)
                {
                    if (@template.Substring(pos).StartsWith(endMarker))
                    {
                        string paramName = @template.Substring(start, pos - start + endMarker.Length);
                        parameterNames.Add(paramName);
                        break;
                    }
                    pos++;
                }
                index = pos+1;
            }

            return parameterNames;
        }

        public Task<TResponse?> DoRequest(TQuestionType question)
        {
            return _aiEngine.DoRequest(question);
        }
    }
}
