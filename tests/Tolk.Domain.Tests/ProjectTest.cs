namespace Tolk.Domain.Tests;

public class ProjectTest
{
    private const string ProjectName = "My test project";
    private const string PhraseEn = "A new phrase";
    private const string PhraseNl = "Een nieuwe tekst";
    private readonly Project _project;

    public ProjectTest()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), ProjectName);
        _project = Project.Create(initialEvent);

        _project.CreatePhrase(PhraseEn);
        _project.UpdateTranslation(PhraseEn, "en", PhraseEn);
        _project.UpdateTranslation(PhraseEn, "nl", PhraseNl);
    }

    [Fact]
    public void ProjectHasVersion()
    {
        Assert.Equal(3, _project.Version);
    }

    [Fact]
    public void ProjectHasUnsavedEventsWithVersions()
    {
        Assert.Collection(_project.UnsavedEvents(),
            e =>
            {
                Assert.IsType<ProjectCreatedEvent>(e);
                Assert.Equal(0, e.Version);
            },
            e =>
            {
                Assert.IsType<PhraseCreatedEvent>(e);
                Assert.Equal(1, e.Version);
            },
            e =>
            {
                Assert.IsType<TranslationUpdatedEvent>(e);
                Assert.Equal(2, e.Version);
            },
            e =>
            {
                Assert.IsType<TranslationUpdatedEvent>(e);
                Assert.Equal(3, e.Version);
            });
    }

    [Fact]
    public void ProjectHasPhraseWithKey()
    {
        Assert.Equal(PhraseEn, _project.Phrases.First().Key);
    }

    [Fact]
    public void ProjectHasPhaseWithTranslations()
    {
        var phrase = _project.Phrases.First();

        var enValue = phrase.Translations.First(t => t.Locale == "en").Value;
        var nlValue = phrase.Translations.First(t => t.Locale == "nl").Value;

        Assert.Equal(PhraseEn, enValue);
        Assert.Equal(PhraseNl, nlValue);
    }
}
