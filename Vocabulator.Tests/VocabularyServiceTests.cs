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
		var columnDescriptions = CsvColumnDescriptions;
		var service = new VocabularyService("nonexistent.csv", columnDescriptions, csvRepo);

		var result = await service.TryLoadAsync();

		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task SaveAsync_CreatesFileWithCorrectContent()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = CsvColumnDescriptions;
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Hallo", [], true),
				Right: new Sentence("Hello", [], false)),
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Welt", [], true),
				Right: new Sentence("World", [], false))
		};

		await service.SaveAsync(vocables);

		Assert.IsTrue(File.Exists(_testFilePath));
		var content = await File.ReadAllTextAsync(_testFilePath);
		Assert.IsTrue(content.Contains("ID"));
		Assert.IsTrue(content.Contains("Key"));
		Assert.IsTrue(content.Contains("Translation"));
		Assert.IsTrue(content.Contains("KeyIsMotherLanguage"));
		Assert.IsTrue(content.Contains("Hallo"));
		Assert.IsTrue(content.Contains("Hello"));
		Assert.IsTrue(content.Contains("Welt"));
		Assert.IsTrue(content.Contains("World"));
	}

	[TestMethod]
	public async Task TryLoadAsync_LoadsPreviouslySavedVocables()
	{
		var csvRepo = new CsvRepo();
		var columnDescriptions = CsvColumnDescriptions;
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo);

		var originalVocables = new List<Vocable>
		{
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Guten Tag", [], true),
				Right: new Sentence("Good day", [], false)),
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Auf Wiedersehen", [], true),
				Right: new Sentence("Goodbye", [], false))
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
		var columnDescriptions = CsvColumnDescriptions;
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("das Haus", [], true),
				Right: new Sentence("the house", [], false))
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
		var columnDescriptions = CsvColumnDescriptions;
		var service = new VocabularyService(_testFilePath, columnDescriptions, csvRepo);

		var vocables = new List<Vocable>
		{
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Ich lerne Deutsch", [], true),
				Right: new Sentence("I learn German", [], false)),
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Du sprichst Englisch", [], true),
				Right: new Sentence("You speak English", [], false)),
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("Er hat ein Auto", [], true),
				Right: new Sentence("He has a car", [], false)),
			new Vocable(
				Guid: Guid.NewGuid(),
				Left: new Sentence("car", [], false),
				Right: new Sentence("Auto", [], true)),
		};

		await service.SaveAsync(vocables);
		var loadedVocables = await service.TryLoadAsync();

		Assert.IsNotNull(loadedVocables);
		Assert.AreEqual(4, loadedVocables.Count);
		
		for (int i = 0; i < vocables.Count; i++)
		{
			Assert.AreEqual(vocables[i].Left.Content, loadedVocables[i].Left.Content);
			Assert.AreEqual(vocables[i].Right.Content, loadedVocables[i].Right.Content);
			Assert.AreEqual(vocables[i].Left.IsMotherLanguage, loadedVocables[i].Left.IsMotherLanguage);
			Assert.AreEqual(vocables[i].Right.IsMotherLanguage, loadedVocables[i].Right.IsMotherLanguage);
		}
	}

	private static List<CsvColumnDescription> CsvColumnDescriptions
	{
		get
		{
			var columnDescriptions = new List<CsvColumnDescription>
			{
				new ("ID", typeof(Guid)),
				new ("Key", typeof(string)),
				new ("Translation", typeof(string)),
				new ("KeyIsMotherLanguage", typeof(bool)),
			};
			return columnDescriptions;
		}
	}
}
