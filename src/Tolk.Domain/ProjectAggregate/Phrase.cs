namespace Tolk.Domain.ProjectAggregate;

public class Phrase : ValueObject
{
    private Phrase(string key, IEnumerable<Translation> translations)
    {
        Key = key;
        Translations = translations;
    }

    public string Key { get; set; }

    public IEnumerable<Translation> Translations { get; }

    public static Phrase Create(string key, IEnumerable<Translation>? translations = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvariantException("Key cannot be empty");

        return new Phrase(key, translations ?? Enumerable.Empty<Translation>());
    }

    public Phrase With(Translation translation)
    {
        return new Phrase(
            Key,
            Translations.Union(new List<Translation> { translation }));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;

        foreach (var translation in Translations) yield return translation; // TODO werkt dit?
    }
}
