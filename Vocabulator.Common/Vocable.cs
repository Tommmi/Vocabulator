namespace Vocabulator.Common;

public record Vocable(Guid Guid, Sentence Left, Sentence Right, List<Word> NewWords);