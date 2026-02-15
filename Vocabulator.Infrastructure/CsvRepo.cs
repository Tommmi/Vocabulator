using System.Globalization;
using System.Text;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Interface;

namespace Vocabulator.Infrastructure;
public class CsvRepo : ICsvRepo
{
	private static readonly string[] PossibleDelimiters = new[] { ";", "\t", "," };

	public async Task<CsvFile?> TryLoadAsync(string filePath)
	{
		if (!File.Exists(filePath))
			return null;

		try
		{
			var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

			// Delimiter-Erkennung (erste Zeile, roh)
			var firstLine = GetFirstPhysicalLine(text);
			var delimiter = DetectDelimiter(firstLine);

			// CSV korrekt parsen (inkl. Quotes, Zeilenumbrüche etc.)
			var allRows = ParseCsv(text, delimiter);
			if (allRows.Count < 1)
				return null;

			var headers = allRows[0];
			var dataRows = allRows.Skip(1).ToList();

			// Spaltentypen ermitteln
			var columnTypes = InferColumnTypes(dataRows);

			var columnDescriptions = headers
				.Select((h, i) => new CsvColumnDescription(h, columnTypes[i]))
				.ToList();

			var rows = new List<CsvRow>();
			foreach (var fields in dataRows)
			{
				var parsedFields = new List<object>();
				for (int i = 0; i < fields.Count; i++)
				{
					parsedFields.Add(ParseValue(fields[i], columnTypes[i]));
				}
				rows.Add(new CsvRow(parsedFields));
			}

			return new CsvFile(columnDescriptions, rows);
		}
		catch
		{
			return null;
		}
	}

	public async Task SaveAsync(CsvFile csvFile, string filePath)
	{
		var sb = new StringBuilder();

		// Header
		sb.AppendLine(string.Join(";", csvFile.ColumnDescriptions.Select(cd => QuoteIfNeeded(cd.HeaderName))));

		// Rows
		foreach (var row in csvFile.Rows)
		{
			var line = string.Join(";", row.Fields.Select(f => QuoteIfNeeded(FormatValue(f))));
			sb.AppendLine(line);
		}

		await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
	}

	// --- CSV Parsing (RFC-konform) ---

	private static List<List<string>> ParseCsv(string text, string delimiter)
	{
		var result = new List<List<string>>();
		var currentRow = new List<string>();
		var currentCell = new StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];

			if (inQuotes)
			{
				if (c == '"')
				{
					// escaped quote?
					bool nextIsQuote = i + 1 < text.Length && text[i + 1] == '"';
					if (nextIsQuote)
					{
						currentCell.Append('"');
						i++; // skip next quote
					}
					else
					{
						inQuotes = false;
					}
				}
				else
				{
					currentCell.Append(c);
				}
			}
			else
			{
				if (c == '"')
				{
					inQuotes = true;
				}
				else if (MatchesDelimiter(text, i, delimiter))
				{
					currentRow.Add(currentCell.ToString());
					currentCell.Clear();
					i += delimiter.Length - 1;
				}
				else if (c == '\r' || c == '\n')
				{
					// handle CRLF or LF
					if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
						i++;
					currentRow.Add(currentCell.ToString());
					currentCell.Clear();
					result.Add(currentRow);
					currentRow = new List<string>();
				}
				else
				{
					currentCell.Append(c);
				}
			}
		}

		// letzte Zeile hinzufügen (auch wenn Datei ohne Newline endet)
		if (currentCell.Length > 0 || currentRow.Count > 0)
		{
			currentRow.Add(currentCell.ToString());
			result.Add(currentRow);
		}

		return result;
	}

	private static bool MatchesDelimiter(string text, int index, string delimiter)
	{
		if (index + delimiter.Length > text.Length) return false;
		return text.Substring(index, delimiter.Length) == delimiter;
	}

	private static string GetFirstPhysicalLine(string text)
	{
		var sb = new StringBuilder();

		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];

			if (c == '\n' || c == '\r')
				break;

			sb.Append(c);
		}

		return sb.ToString();
	}

	private static string DetectDelimiter(string line)
	{
		return PossibleDelimiters
			.OrderByDescending(d => line.Count(c => c.ToString() == d))
			.FirstOrDefault() ?? ";";
	}

	private static List<Type> InferColumnTypes(List<List<string>> rows)
	{
		if (rows.Count == 0)
			return new List<Type>();

		int columnCount = rows.Max(r => r.Count);
		var columnTypes = new List<Type>(Enumerable.Repeat(typeof(string), columnCount));

		for (int col = 0; col < columnCount; col++)
		{
			var values = rows
				.Where(r => r.Count > col)
				.Select(r => r[col])
				.Where(v => !string.IsNullOrWhiteSpace(v))
				.ToList();

			columnTypes[col] = InferType(values);
		}

		return columnTypes;
	}

	private static Type InferType(List<string> values)
	{
		if (values.Count == 0)
			return typeof(string);

		bool allBool = values.All(v => bool.TryParse(v, out _)
			|| v.Equals("0") || v.Equals("1")
			|| v.Equals("yes", StringComparison.OrdinalIgnoreCase)
			|| v.Equals("no", StringComparison.OrdinalIgnoreCase));
		if (allBool)
			return typeof(bool);

		bool allInt = values.All(v => int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out _));
		if (allInt)
			return typeof(int);

		bool allDouble = values.All(v => double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out _));
		if (allDouble)
			return typeof(double);

		bool allGuid = values.All(v => Guid.TryParse(v, out _));
		if (allGuid)
			return typeof(Guid);

		return typeof(string);
	}

	private static object ParseValue(string value, Type type)
	{
		if (string.IsNullOrWhiteSpace(value))
			return "";

		if (type == typeof(bool))
		{
			if (bool.TryParse(value, out var b))
			{
				return b;
			}

			if (value == "1" 
			    || value.Equals("yes", StringComparison.OrdinalIgnoreCase) 
			    || value.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
			    || value.Equals("WAHR", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return false;
		}

		if (type == typeof(int))
		{
			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;
		}

		if (type == typeof(double))
		{
			return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0.0;
		}

		if (type == typeof(Guid))
		{
			return Guid.TryParse(value, out var d) ? d : Guid.NewGuid();
		}

		return value;
	}

	private static string FormatValue(object? value)
	{
		if (value == null) return "";
		if (value is bool b) return b ? "true" : "false";
		if (value is double d) return d.ToString(CultureInfo.InvariantCulture);
		if (value is int i) return i.ToString(CultureInfo.InvariantCulture);
		if (value is Guid g) return g.ToString();
		return value?.ToString()??"";
	}

	private static string QuoteIfNeeded(string? value)
	{
		if (value == null)
		{
			return "";
		}

		bool needsQuote = value.Contains(';') || value.Contains('\t') || value.Contains('\n') || value.Contains('"');
		
		if (!needsQuote)
		{
			return value;
		}

		var escaped = value.Replace("\"", "\"\"");

		return $"\"{escaped}\"";
	}
}
