namespace Tolk.Domain.Tests;

public class PhraseTest
{
    [Fact]
    public void KeyTest()
    {
        Assert.Throws<InvariantException>(() => Phrase.Create("", new List<Translation>()));
    }

    [Fact]
    public void WithTranslationTest()
    {
        var phrase = Phrase.Create("Test", new List<Translation>());

        var newPhrase = phrase.With(Translation.Create("nl_NL", "Hoi"));

        Assert.Single(newPhrase.Translations);
    }

    [Fact]
    public void CreatePhaseTest()
    {
        var key = "A new phrase";
        var translation = "Een nieuwe tekst";
        
        var initialEvent = new ProjectCreatedEvent(new Guid(), "My test project");
        var project = Project.Create(initialEvent);
        
        project.CreatePhrase(key);
        
        Assert.Equal(key, project.Phrases.First().Key);
        
        project.UpdateTranslation(key, "en", key);
        project.UpdateTranslation(key, "nl", translation);

        var phrase = project.Phrases.First();

        var enValue = phrase.Translations.First(t => t.Locale == "en").Value;
        var nlValue = phrase.Translations.First(t => t.Locale == "nl").Value;
        
        Assert.Equal(key, enValue);
        Assert.Equal(translation, nlValue);
        Assert.Equal(4, project.Version);
    }

    [Fact]
    public void UpdateTranslationForNonexistentPhrase()
    {
        var initialEvent = new ProjectCreatedEvent(new Guid(), "My test project");
        var project = Project.Create(initialEvent);
        
        Assert.Throws<InvariantException>(() => project.UpdateTranslation("key", "en", "value"));


    }
}
