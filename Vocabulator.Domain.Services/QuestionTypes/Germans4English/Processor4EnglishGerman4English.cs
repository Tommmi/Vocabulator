using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.Germans4English
{
    public class Processor4EnglishGerman4English : ProcessorBase<WordAnswer, Processor4EnglishGerman4English.QuestionType>
    {
        public class QuestionType : Question
        {
            public QuestionType(QuestionTemplate template, string word) 
                : base(
                    template, 
                    parameters:
                    [
	                    new("WORD", word),
	                    new("deutsche", "englische"),
	                    new("englische", "deutsche"),
	                    new("Muttersprache", "englisch")
					])
            {

            }
        }

        public Processor4EnglishGerman4English(IAiEngineFactory aiEngineFactory, string questionFilePath) 
            : base(
                aiEngineFactory: aiEngineFactory, 
                questionFilePath: questionFilePath,
                className:nameof(Processor4EnglishGerman4English))
        {
        }

        public override async Task<WordAnswer?> LoadAnswer(string word)
        {
            var question = new QuestionType(_aiProcessor.QuestionTemplate, word);
            return await _aiProcessor.DoRequest(question);
        }
    }
}
