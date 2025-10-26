using Vocabulator.Common.Csv;

namespace Vocabulator.Domain.Interface
{
	public interface ICsvRepo
	{
		Task<CsvFile?> TryLoadAsync(string filePath);
		Task SaveAsync(CsvFile csvFile, string filePath);
	}
}
