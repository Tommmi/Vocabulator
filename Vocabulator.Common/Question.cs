namespace Vocabulator.Common;

public class Question
{
    public QuestionTemplate Template { get; }
    public List<KeyValuePair<string, string>> Parameters { get; }

    public string Text
    {
        get
        {
            string question = Template.Template;
            foreach (var parameter in Parameters)
            {
                question = question.Replace($"PARAM({parameter.Key})PARAM", parameter.Value);
            }
            return question;
        }
    }

    public Question(QuestionTemplate template, List<KeyValuePair<string,string>> parameters)
    {
        Template = template;
        Parameters = parameters;
        ValidateParameters();
    }

    private void ValidateParameters()
    {
        foreach(var parameter in Parameters)
        {
            if(Template.ParameterNames.All(p => p != parameter.Key))
            {
                throw new ArgumentException($"{parameter.Key} doesn't exist in template");
            }
        }
        foreach(var parameter in Template.ParameterNames)
        {
            if (Parameters.All(p=>p.Key!=parameter))
            {
                throw new ArgumentException($"{parameter} doesn't exist in parameter list");
            }
        }
    }
}