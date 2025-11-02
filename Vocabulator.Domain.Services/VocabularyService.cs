using System.Text.RegularExpressions;
using Vocabulator.Common;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Interface;
// ReSharper disable InconsistentNaming

namespace Vocabulator.Domain.Services
{
	public class VocabularyService : IVocabularyService
	{
		#region fields

		private readonly string _filepath;
		private readonly List<CsvColumnDescription> _columnDescriptions;
		private readonly ICsvRepo _csvRepo;
		private const int _colNbGuid = 0;
		private const int _colNbLeft = 1;
		private const int _colNbRight = 2;
		private const int _colNbKeyIsMotherLanguage = 3;

		#endregion

		#region constructor

		public VocabularyService(
			string filepath,
			List<CsvColumnDescription> columnDescriptions,
			ICsvRepo csvRepo)
		{
			_filepath = filepath;
			_columnDescriptions = columnDescriptions;
			_csvRepo = csvRepo;
		}

		#endregion

		#region IVocabularyService

		public async Task<List<Vocable>?> TryLoadAsync()
		{
			var csvFile = await _csvRepo.TryLoadAsync(filePath: _filepath);
			if(csvFile == null)
			{
				return null;
			}

			var vocables = csvFile.Rows.Select(row =>
				new Vocable(
					Guid: (Guid)row.Fields[_colNbGuid],
					Left: CreateSentence(row, _colNbLeft),
					Right: CreateSentence(row, _colNbRight))
				)
				.ToList();
			return vocables;
		}

		private Sentence CreateSentence(CsvRow row, int colNb)
		{
			bool isKeyMotherLanguage = (bool)row.Fields[_colNbKeyIsMotherLanguage];
			bool isColKeyCol = colNb == _colNbLeft;
			bool isColMotherLanguage = isColKeyCol ? isKeyMotherLanguage :  !isKeyMotherLanguage;

			return new Sentence(
				Content: row.Fields[colNb].ToString() ?? "",
				Words: ParseWords(fieldTxt: row.Fields[colNb], isMotherLanguage: isColMotherLanguage),
				IsMotherLanguage: isColMotherLanguage);
		}

		public Vocable CreateVocable(string leftSentence, string rightSentence, bool isLeftMotherLanguage)
		{
			return new Vocable(
				Guid: Guid.NewGuid(),
				Left: CreateSentence(leftSentence, isMotherLanguage: isLeftMotherLanguage),
				Right: CreateSentence(rightSentence, isMotherLanguage: !isLeftMotherLanguage));
		}

		private Sentence CreateSentence(string sentence, bool isMotherLanguage)
		{
			return new Sentence(
				Content: sentence,
				Words: ParseWords(fieldTxt: sentence, isMotherLanguage:isMotherLanguage),
				IsMotherLanguage: isMotherLanguage);
		}

		public async Task SaveAsync(List<Vocable> vocables)
		{
			await _csvRepo.SaveAsync(
				new CsvFile(
					columnDescriptions: _columnDescriptions,
					rows: vocables
						.Select(v => new CsvRow(
							fields: [v.Guid, v.Left.Content, v.Right.Content, v.Left.IsMotherLanguage]))
						.ToList()
				),
				filePath: _filepath);
		}

		#endregion

		#region private

		private Word[] ParseWords(object fieldTxt, bool isMotherLanguage)
		{
			string text = (string)fieldTxt;
			if (!isMotherLanguage)
			{
				return ExtractWords(input: text).Select(x => new Word(Token: x.ToLower())).ToArray();
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

			input = Regex.Replace(input, @"\[.*?\]", string.Empty);

			var matches = Regex.Matches(input, @"\p{L}+");

			var words = new List<string>(matches.Count);
			foreach (Match match in matches)
			{
				words.Add(match.Value);
			}

			return words;
		}

		#endregion
	}
}
