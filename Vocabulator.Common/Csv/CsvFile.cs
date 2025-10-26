
namespace Vocabulator.Common.Csv
{
	public class CsvFile
	{
		public List<CsvColumnDescription> ColumnDescriptions { get; }
		public List<CsvRow> Rows { get; }

		public CsvFile(List<CsvColumnDescription> columnDescriptions, List<CsvRow> rows)
		{
			ColumnDescriptions = columnDescriptions;
			Rows = rows;
		}
	}
}
