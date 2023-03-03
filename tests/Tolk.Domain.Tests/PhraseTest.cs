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
    public void UpdateTranslationForNonexistentPhrase()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), "My test project");
        var project = Project.Create(initialEvent);

        Assert.Throws<InvariantException>(() => project.UpdateTranslation("key", "en", "value"));
    }
}
