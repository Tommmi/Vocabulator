namespace Vocabulator.Common;

public class TrainingDataItem : ITrainingDataItem
{
    public string Left { get; }
    public string Right { get; }

    public TrainingDataItem(string left, string right)
    {
        Left = left;
        Right = right;
    }
}