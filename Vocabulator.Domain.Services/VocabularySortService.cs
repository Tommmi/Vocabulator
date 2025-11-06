using Vocabulator.Common;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Domain.Services
{
	public class VocabularySortService : IVocabularySortService
	{
		public List<Vocable> Sort(List<Vocable> vocables)
		{
			HashSet<Word> knownWords = new();

			List<Vocable> targetList = new ();
			var remainingVocables = vocables.ToList();
			remainingVocables.Reverse();

			int oldCountTargetList;

			while(remainingVocables.Count > 0)
			{
				ExtractRootVocables();

				if(remainingVocables.Count > 0)
				{
					ExtractOneVocable(idxInRemainingVocables: remainingVocables.Count-1);
				}
			}

			return targetList;

			void ExtractRootVocables()
			{
				ExtractRootVocablesInternal(maxSingleNewWords: 1);
				ExtractRootVocablesInternal(maxSingleNewWords: 2);
			}

			void ExtractRootVocablesInternal(int maxSingleNewWords)
			{
				do
				{
					oldCountTargetList = targetList.Count;

					for (int idx = remainingVocables.Count - 1; idx >= 0; idx--)
					{
						var vocable = remainingVocables[idx];
						var sentence = GetForeignSentence(vocable);
						bool isRoot = sentence.Words.Count(word => !knownWords.Contains(word)) < maxSingleNewWords + 1;

						if (isRoot)
						{
							ExtractOneVocable(idx);
						}
					}
				}
				while (targetList.Count != oldCountTargetList);
			}

			void ExtractOneVocable(int idxInRemainingVocables)
			{
				var vocable = remainingVocables[idxInRemainingVocables];
				var sentence = GetForeignSentence(vocable);
				targetList.Add(vocable);

				vocable.NewWords.Clear();

				foreach (var word in sentence.Words)
				{
					bool isKnown = knownWords.Contains(word);
					if(!isKnown)
					{
						vocable.NewWords.Add(word);
					}
					knownWords.Add(word);
				}

				remainingVocables.RemoveAt(idxInRemainingVocables);
			}
		}

		public (int line, Word word)? TryFindUntranslatedWord(List<Vocable> vocables)
		{
			HashSet<Word> knownWords = new ();
			int lineNb = 0;

			foreach (var vocable in vocables)
			{
				var sentence = GetForeignSentence(vocable);
				var result = TryFindUntranslatedVocable(lineNb, sentence, knownWords);

				if(result != null)
				{
					return result;
				}

				foreach (var w in sentence.Words)
				{
					knownWords.Add(w);
				}

				lineNb++;
			}

			return null;
		}

		private (int line, Word word)? TryFindUntranslatedVocable(int lineNb, Sentence sentence, HashSet<Word> knownWords)
		{
			var unKnownWords = sentence.Words.Where(w=> !knownWords.Contains(w)).ToArray();
			if(unKnownWords.Length > 1)
			{
				int rndInt = Random.Shared.Next(minValue:0, maxValue: unKnownWords.Length - 1);
				var rndWord = unKnownWords[rndInt];
				return (line: lineNb, word: rndWord);
			}
			return null;
		}

		private static Sentence GetForeignSentence(Vocable vocable)
		{
			if(vocable.Left.IsMotherLanguage)
			{
				return vocable.Right;
			}

			return vocable.Left;
		}
	}
}
