using Vocabulator.Common;
using Vocabulator.Domain.Services;

namespace Vocabulator.Tests;

[TestClass]
public class VocabularySortTests
{
	[TestMethod]
	public void TestSortVocabulary()
	{
		var sortService = new VocabularySortService();
		var vocables = new List<Vocable>()
		{
			new (Guid.NewGuid(), new Sentence("",[],true),new Sentence("b", [new ("b")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("q w r", [new ("q"),new ("w"),new ("r")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("d", [new ("d")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("a b", [new ("a"),new ("b")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("a", [new ("a")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("t i z y", [new ("t"),new ("i"),new ("z"),new ("y")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("e f", [new ("e"),new ("f")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("b o", [new ("b"),new ("o")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("c", [new ("c")],false)),
			new (Guid : Guid.NewGuid(), new Sentence("",[],true),new Sentence("a t i", [new ("a"),new ("t"),new ("i")],false)),
		};

		var result = sortService.Sort(vocables);

		foreach(var item in result)
		{
			Console.WriteLine(item.Right.Content);
		}
	}

}