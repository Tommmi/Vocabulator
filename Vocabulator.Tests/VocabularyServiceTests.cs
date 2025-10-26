using Vocabulator.Common;
using Vocabulator.Common.Csv;
using Vocabulator.Domain.Services;
using Vocabulator.Infrastructure;

namespace Vocabulator.Tests;

[TestClass]
public class VocabularyServiceTests
{
	private string _testFilePath = "";

	[TestInitialize]
	public void Initialize()
	{
		_testFilePath = $"test_vocabulary_{Guid.NewGuid()}.csv";
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (File.Exists(_testFilePath))
		{
			File.Delete(_testFilePath);
		}
	}

	[TestMethod]
	public async Task TryLoadAsync_FileDoesNotExist_ReturnsNull()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = new List<CsvColumnDescription>
		{
			new("German", typeof(string)),
			new("English", typeof(string))
		};
		var service = new VocabularyService("nonexistent.csv", columnDescriptions, csvRepo, 1);

		var result = await service.TryLoadAsync();

		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task SaveAsync_CreatesFileWithCorrectContent()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = new List<CsvColumnDescription>
		{
			new("German", typeof(string)),
			new("English", typeof(string))
		};
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo, 1);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Left: new Sentence("Hallo", [], false),
				Right: new Sentence("Hello", [], true)),
			new Vocable(
				Left: new Sentence("Welt", [], false),
				Right: new Sentence("World", [], true))
		};

		await service.SaveAsync(vocables);

		Assert.IsTrue(File.Exists(_testFilePath));
		var content = await File.ReadAllTextAsync(_testFilePath);
		Assert.IsTrue(content.Contains("German"));
		Assert.IsTrue(content.Contains("English"));
		Assert.IsTrue(content.Contains("Hallo"));
		Assert.IsTrue(content.Contains("Hello"));
		Assert.IsTrue(content.Contains("Welt"));
		Assert.IsTrue(content.Contains("World"));
	}

	[TestMethod]
	public async Task TryLoadAsync_LoadsPreviouslySavedVocables()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = new List<CsvColumnDescription>
		{
			new("German", typeof(string)),
			new("English", typeof(string))
		};
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo, 1);

		var originalVocables = new List<Vocable>
		{
			new Vocable(
				Left: new Sentence("Guten Tag", [], false),
				Right: new Sentence("Good day", [], true)),
			new Vocable(
				Left: new Sentence("Auf Wiedersehen", [], false),
				Right: new Sentence("Goodbye", [], true))
		};

		await service.SaveAsync(originalVocables);

		var loadedVocables = await service.TryLoadAsync();

		Assert.IsNotNull(loadedVocables);
		Assert.AreEqual(2, loadedVocables.Count);
		Assert.AreEqual("Guten Tag", loadedVocables[0].Left.Content);
		Assert.AreEqual("Good day", loadedVocables[0].Right.Content);
		Assert.AreEqual("Auf Wiedersehen", loadedVocables[1].Left.Content);
		Assert.AreEqual("Goodbye", loadedVocables[1].Right.Content);
	}

	[TestMethod]
	public async Task TryLoadAsync_ParsesWordsFromForeignLanguage()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = new List<CsvColumnDescription>
		{
			new("German", typeof(string)),
			new("English", typeof(string))
		};
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo, foreignLanguageColumnNumber: 1);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Left: new Sentence("das Haus", [], false),
				Right: new Sentence("the house", [], true))
		};

		await service.SaveAsync(vocables);
		var loadedVocables = await service.TryLoadAsync();

		Assert.IsNotNull(loadedVocables);
		Assert.AreEqual(1, loadedVocables.Count);
		Assert.AreEqual(0, loadedVocables[0].Left.Words.Length);
		Assert.IsTrue(loadedVocables[0].Left.IsMotherLanguage);
		Assert.IsFalse(loadedVocables[0].Right.IsMotherLanguage);
		Assert.AreEqual(2,loadedVocables[0].Right.Words.Length);
	}

	[TestMethod]
	public async Task SaveAndLoad_MultipleVocables_PreservesData()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = new List<CsvColumnDescription>
		{
			new("German", typeof(string)),
			new("English", typeof(string))
		};
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo, 1);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Left: new Sentence("Ich lerne Deutsch", [], false),
				Right: new Sentence("I learn German", [], true)),
			new Vocable(
				Left: new Sentence("Du sprichst Englisch", [], false),
				Right: new Sentence("You speak English", [], true)),
			new Vocable(
				Left: new Sentence("Er hat ein Auto", [], false),
				Right: new Sentence("He has a car", [], true))
		};

		await service.SaveAsync(vocables);
		var loadedVocables = await service.TryLoadAsync();

		Assert.IsNotNull(loadedVocables);
		Assert.AreEqual(3, loadedVocables.Count);
		
		for (int i = 0; i < vocables.Count; i++)
		{
			Assert.AreEqual(vocables[i].Left.Content, loadedVocables[i].Left.Content);
			Assert.AreEqual(vocables[i].Right.Content, loadedVocables[i].Right.Content);
		}
	}
}
