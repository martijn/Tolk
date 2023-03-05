using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;


namespace Tolk.E2ETests;

public class ProjectTests : PlaywrightTest
{
    private IAPIRequestContext _request = null!;
    private bool _functionAppAlreadyRunning;

    [SetUp]
    public async Task Setup()
    {
        _functionAppAlreadyRunning = Process.GetProcessesByName("func").Any();
        if (!_functionAppAlreadyRunning) await StartFunctionApp();
        
        _request = await this.Playwright.APIRequest.NewContextAsync(new() {
            BaseURL = "http://localhost:7071"
        });
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _request.DisposeAsync();

        if (!_functionAppAlreadyRunning) StopFunctionApp();
    }

    [Test]
    public async Task ProjectIntegrationTest()
    {
        var projectName = $"ProjectIntegrationTest {DateTime.UtcNow}";
        
        // CreateProject
        var createResponse = await _request.PostAsync("/api/CreateProject", new()
        {
            DataObject = new Dictionary<string, string>() { { "Name", projectName } }
        });

        var projectId = (await createResponse.JsonAsync()).Value.GetProperty("Id").GetString();
        Assert.That(projectId, Is.Not.Null);

        var projectState1 = await GetProject(projectId);
        Assert.That(projectState1.GetProperty("Name").GetString(), Is.EqualTo(projectName));
        await TestContext.Out.WriteLineAsync(projectState1.ToString());
        
        // CreatePhrase, AddTranslation
        var createPhraseResponse = await _request.PostAsync("/api/CreatePhrase", new()
        {
            DataObject = new Dictionary<string, string>()
            {
                { "projectId", projectId },
                { "name", "A phrase" }
            }
        });
        Assert.That(createPhraseResponse.Ok, Is.True);
        var updateTranslationEnResponse = await _request.PostAsync("/api/UpdateTranslation", new()
        {
            DataObject = new Dictionary<string, string>()
            {
                { "projectId", projectId },
                { "phraseKey", "A phrase" },
                { "locale", "en" },
                { "value", "A phrase" }
            }
        });
        Assert.That(updateTranslationEnResponse.Ok, Is.True);
        var updateTranslationNlResponse = await _request.PostAsync("/api/UpdateTranslation", new()
        {
            DataObject = new Dictionary<string, string>()
            {
                { "projectId", projectId },
                { "phraseKey", "A phrase" },
                { "locale", "nl" },
                { "value", "Een zin" }
            }
        });
        Assert.That(updateTranslationNlResponse.Ok, Is.True);
        
        var projectState2 = await GetProject(projectId);
        Assert.That(projectState2.GetProperty("Phrases").GetArrayLength(), Is.EqualTo(1));
        
        // ArchiveProject
        
        var archiveResponse = await _request.PostAsync("/api/ArchiveProject", new()
        {
            DataObject = new Dictionary<string, string>() { { "projectId", projectId } }
        });
        Assert.That(archiveResponse.Ok, Is.True);
        
        // Assert end state
        
        var projectState3 = await GetProject(projectId);
        
        await TestContext.Out.WriteLineAsync(projectState3.ToString());
        
        Assert.That(projectState3.GetProperty("Archived").GetBoolean(), Is.True);

        var translations = projectState3.GetProperty("Phrases")[0].GetProperty("Translations");
        Assert.That(translations.GetArrayLength(), Is.EqualTo(2));
        Assert.That(translations[0].GetProperty("Locale").GetString(), Is.EqualTo("en"));
        Assert.That(translations[0].GetProperty("Value").GetString(), Is.EqualTo("A phrase"));
        Assert.That(translations[1].GetProperty("Locale").GetString(), Is.EqualTo("nl"));
        Assert.That(translations[1].GetProperty("Value").GetString(), Is.EqualTo("Een zin"));
        
        // TODO Geen Create Phrase op ArchivedProject + Assert status and Phrases length
    }

    private async Task<JsonElement> GetProject(string projectId)
    {
        var getResponse1 = await _request.GetAsync($"/api/GetProject/{projectId}");
        Assert.That(getResponse1.Ok, Is.True);
        
        return (await getResponse1.JsonAsync())!.Value;
    }

    private async Task StartFunctionApp()
    {
        var funcProcess = new Process();

        var rootDir = Path.GetFullPath(@"../../../../..");
        var e2eAppBinPath = Path.Combine(rootDir, @"src/Tolk.FunctionApp/bin/Debug/net7.0");
        string e2eHostJson = Directory.GetFiles(e2eAppBinPath, "host.json", SearchOption.AllDirectories).FirstOrDefault();

        if (e2eHostJson == null)
        {
            throw new InvalidOperationException($"Could not find a built worker app under '{e2eAppBinPath}'");
        }

        var e2eAppPath = Path.GetDirectoryName(e2eHostJson);

        var cliPath = "/usr/local/bin/func";

        if (!File.Exists(cliPath))
        {
            throw new InvalidOperationException($"Could not find '{cliPath}'.");
        }
        
        funcProcess.StartInfo.UseShellExecute = false;
        funcProcess.StartInfo.RedirectStandardError = true;
        funcProcess.StartInfo.RedirectStandardOutput = true;
        funcProcess.StartInfo.CreateNoWindow = true;
        funcProcess.StartInfo.WorkingDirectory = e2eAppPath;
        funcProcess.StartInfo.FileName = cliPath;
        funcProcess.StartInfo.ArgumentList.Add("host");
        funcProcess.StartInfo.ArgumentList.Add("start");
        funcProcess.StartInfo.ArgumentList.Add("--csharp");
        funcProcess.StartInfo.ArgumentList.Add("--verbose");
        
        funcProcess.Start();

        // TODO Wait until app is responsive, with timeout
        await Task.Delay(2500);
    }

    private void StopFunctionApp()
    {
        foreach (var func in Process.GetProcessesByName("func"))
        {
            try
            {
                func.Kill();
            }
            catch
            {
                // Best effort
            }
        }
    }
}
