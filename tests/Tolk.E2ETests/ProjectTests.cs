using System.Diagnostics;
using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Tolk.E2ETests;

[NonParallelizable]
public class ProjectTests : PlaywrightTest
{
    private bool _functionAppAlreadyRunning;
    private string? _projectId;
    private IAPIRequestContext _request = null!;
    const string PhraseName = "A phrase";

    [OneTimeSetUp]
    public async Task StartFunctionApp()
    {
        _functionAppAlreadyRunning = Process.GetProcessesByName("func").Any();
        if (!_functionAppAlreadyRunning) await FunctionAppHelpers.StartFunctionApp();
    }

    [OneTimeTearDown]
    public void StopFunctionApp()
    {
        if (!_functionAppAlreadyRunning) FunctionAppHelpers.StopFunctionApp();
    }

    [SetUp]
    public async Task Setup()
    {
        _request = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = "http://localhost:7071"
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        await _request.DisposeAsync();
    }

    [Test]
    [Order(0)]
    public async Task CreateProject()
    {
        var projectName = $"ProjectIntegrationTest {DateTime.UtcNow}";

        var createResponse = await _request.PostAsync("/api/CreateProject", new APIRequestContextOptions
        {
            DataObject = new Dictionary<string, string> { { "Name", projectName } }
        });
        Assert.That(createResponse.Ok, Is.True);

        _projectId = (await createResponse.JsonAsync())!.Value.GetProperty("Id").GetString()!;
        Assert.That(_projectId, Is.Not.Null);

        var projectState = await GetProject(_projectId);
        Assert.That(projectState.GetProperty("Name").GetString(), Is.EqualTo(projectName));
    }

    [Test]
    [Order(1)]
    public async Task CreatePhrase()
    {
        var phraseData = new Dictionary<string, string>
        {
            { "projectId", _projectId! },
            { "name", PhraseName }
        };
        var createPhraseResponse = await _request.PostAsync("/api/CreatePhrase",
            new APIRequestContextOptions { DataObject = phraseData });
        Assert.That(createPhraseResponse.Ok, Is.True);
        
        var projectState = await GetProject(_projectId!);
        var phrases = projectState.GetProperty("Phrases");
        Assert.That(phrases.GetArrayLength(), Is.EqualTo(1));
        Assert.That(phrases[0].GetProperty("Key").GetString(), Is.EqualTo(PhraseName));
    }

    [Test]
    [Order(2)]
    public async Task AddTranslations()
    {
        const string enLocale = "en";
        const string nlLocale = "nl";
        const string enValue = "A phrase";
        const string nlValue = "Een zin";

        var enTranslationData = new Dictionary<string, string>
        {
            { "projectId", _projectId! },
            { "phraseKey", PhraseName },
            { "locale", enLocale },
            { "value", enValue }
        };
        var updateEnTranslationResponse = await _request.PostAsync("/api/UpdateTranslation",
            new APIRequestContextOptions { DataObject = enTranslationData });
        Assert.That(updateEnTranslationResponse.Ok, Is.True);

        var nlTranslationData = new Dictionary<string, string>
        {
            { "projectId", _projectId! },
            { "phraseKey", PhraseName },
            { "locale", nlLocale },
            { "value", nlValue }
        };
        var updateNlTranslationResponse = await _request.PostAsync("/api/UpdateTranslation",
            new APIRequestContextOptions { DataObject = nlTranslationData });
        Assert.That(updateNlTranslationResponse.Ok, Is.True);

        var projectState = await GetProject(_projectId!);
        var phrases = projectState.GetProperty("Phrases");
        var translations = phrases[0].GetProperty("Translations");
        Assert.That(translations.GetArrayLength(), Is.EqualTo(2));

        var enTranslation = translations[0];
        var nlTranslation = translations[1];

        Assert.Multiple(() =>
        {
            Assert.That(enTranslation.GetProperty("Locale").GetString(), Is.EqualTo(enLocale));
            Assert.That(enTranslation.GetProperty("Value").GetString(), Is.EqualTo(enValue));
            Assert.That(nlTranslation.GetProperty("Locale").GetString(), Is.EqualTo(nlLocale));
            Assert.That(nlTranslation.GetProperty("Value").GetString(), Is.EqualTo(nlValue));
        });
    }
    
    [Test]
    [Order(3)]
    public async Task ArchiveProject()
    {
        var archiveResponse = await _request.PostAsync("/api/ArchiveProject", new APIRequestContextOptions
        {
            DataObject = new Dictionary<string, string> { { "projectId", _projectId! } }
        });
        Assert.That(archiveResponse.Ok, Is.True);

        var projectState = await GetProject(_projectId!);
        Assert.That(projectState.GetProperty("Archived").GetBoolean(), Is.True);
    }


    [Test]
    [Order(4)]
    public async Task CannotCreatePhraseOnArchivedProject()
    {
        var createPhraseResponse = await _request.PostAsync("/api/CreatePhrase", new APIRequestContextOptions
        {
            DataObject = new Dictionary<string, string>
            {
                { "projectId", _projectId! },
                { "name", "A phrase" }
            }
        });
        Assert.That(createPhraseResponse.Ok, Is.False);

        // TODO return and verify error message and status code
    }

    private async Task<JsonElement> GetProject(string projectId)
    {
        var getResponse1 = await _request.GetAsync($"/api/GetProject/{projectId}");
        Assert.That(getResponse1.Ok, Is.True);

        return (await getResponse1.JsonAsync())!.Value;
    }
}
