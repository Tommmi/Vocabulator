namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord;

public class ContextEntry
{
    public string Context { get; set; }
    public string[] Translations { get; set; }
    public Example[] Examples { get; set; }
}