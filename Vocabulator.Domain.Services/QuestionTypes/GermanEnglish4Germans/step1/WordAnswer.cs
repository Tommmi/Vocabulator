using Vocabulator.Common;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanEnglish4Germans.step1
{
    public class WordAnswer : IResult
    {
        public Factor[] Factors { get; set; }
        public Idiomatic[] Idiomatics { get; set; }
        public List<ITrainingDataItem> GetTrainingData(string word)
        {
            List<ITrainingDataItem> result = new();
            foreach (var factor in Factors)
            {
                foreach(var variation in factor.FactorVariations)
                {
                    result.Add(new TrainingDataItem(
                        left: $"{word}({variation.Variation})", 
                        right: $"{variation.Translations
                            .Select(t => t.TranslationText)
                            .StringJoin(delimiter: ",")}"));

                    foreach(var translation in variation.Translations)
                    {
                        result.Add(new TrainingDataItem(
                            left: $"{translation.Example.Translation}",
                            right: $"{translation.Example.Source}"));
                    }
                }
            }

            foreach (var idiomatic in Idiomatics)
            {
                result.Add(new TrainingDataItem(
                    left: $"{idiomatic.Source}",
                    right: $"{idiomatic.Translation}"));
            }

            return result;
        }
    }
}
