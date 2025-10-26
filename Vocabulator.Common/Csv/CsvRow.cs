namespace Vocabulator.Common.Csv;

public class CsvRow
{
	public List<object> Fields { get; }

	public CsvRow(List<object> fields)
	{
		Fields = fields;
	}
}