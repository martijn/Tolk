namespace Tolk.Domain.ProjectAggregate.Events;

public class TranslationUpdatedEvent : Event
{
    public TranslationUpdatedEvent(Guid id, string phraseKey, string locale, string value) : base(id)
    {
        PhraseKey = phraseKey;
        Locale = locale;
        Value = value;
    }

    public string PhraseKey { get; }
    public string Locale { get; }
    public string Value { get; }
}
