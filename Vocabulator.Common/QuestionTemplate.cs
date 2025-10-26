
namespace Vocabulator.Common
{
    public class QuestionTemplate
    {
        public string TemplateName { get; }
        public List<string> ParameterNames { get; }
        public string Template { get;  }

        public QuestionTemplate(string templateName, string template, List<string> parameterNames)
        {
            TemplateName = templateName;
            ParameterNames = parameterNames;
            Template = template;
        }
    }
}
