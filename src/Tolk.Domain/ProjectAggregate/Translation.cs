namespace Tolk.Domain.ProjectAggregate;

public class Translation : ValueObject
{
    private Translation(string locale, string value)
    {
        Locale = locale;
        Value = value;
    }

    public string Locale { get; }
    public string Value { get; }

    public static Translation Create(string locale, string value)
    {
        return new(locale, value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Locale;
        yield return Value;
    }
}
