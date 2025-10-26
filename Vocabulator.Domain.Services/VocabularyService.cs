using System.Text.RegularExpressions;
using Vocabulator.Common;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Domain.Services
{
	public class VocabularyService : IVocabularyService
	{
		private readonly string _filepath;
		private readonly List<CsvColumnDescription> _columnDescriptions;
		private readonly ICsvRepo _csvRepo;
		private readonly int _foreignLanguageColumnNumber;

		public VocabularyService(
			string filepath,
			List<CsvColumnDescription> columnDescriptions,
			ICsvRepo csvRepo,
			int foreignLanguageColumnNumber)
		{
			_filepath = filepath;
			_columnDescriptions = columnDescriptions;
			_csvRepo = csvRepo;
			_foreignLanguageColumnNumber = foreignLanguageColumnNumber;
		}

		public async Task<List<Vocable>?> TryLoadAsync()
		{
			var csvFile = await _csvRepo.TryLoadAsync(filePath: _filepath);
			if(csvFile == null)
			{
				return null;
			}

			int colNbLeft = 0;
			int colNbRight = 1;

			var vocables = csvFile.Rows.Select(row =>
				new Vocable(
					Left: new Sentence(
						Content: row.Fields[colNbLeft].ToString()??"",
						Words: ParseWords(fieldTxt: row.Fields[colNbLeft], colNb: colNbLeft),
						IsMotherLanguage: _foreignLanguageColumnNumber != colNbLeft),
					Right: new Sentence(
						Content: row.Fields[colNbRight].ToString() ?? "",
						Words: ParseWords(fieldTxt: row.Fields[colNbLeft], colNb: colNbRight),
						IsMotherLanguage: _foreignLanguageColumnNumber != colNbRight)))
				.ToList();
			return vocables;
		}

		private Word[] ParseWords(object fieldTxt, int colNb)
		{ 
			string text = (string)fieldTxt;
			if(colNb == _foreignLanguageColumnNumber)
			{
				return ExtractWords(input: text).Select(x => new Word(Token:x)).ToArray();
			}
			else
			{
				return [];
			}
		}

		private static List<string> ExtractWords(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return new List<string>();

			var matches = Regex.Matches(input, @"\p{L}+");

			var words = new List<string>(matches.Count);
			foreach (Match match in matches)
			{
				words.Add(match.Value);
			}

			return words;
		}


		public async Task SaveAsync(List<Vocable> vocables)
		{
			await _csvRepo.SaveAsync(
				new CsvFile(
					columnDescriptions: _columnDescriptions,
					rows: vocables
							.Select(v => new CsvRow(
								fields: [v.Left.Content, v.Right.Content]))
							.ToList()
				),
				filePath: _filepath);
		}
	}
}
