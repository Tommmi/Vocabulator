namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord;

public class RootObject
{
    public string Word { get; set; }
    public ContextEntry[] Translations { get; set; }
}