using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.Domain.ProjectAggregate;

public class Project : Aggregate
{
    public Project(Guid id, IEnumerable<IEvent> events) : base(id, events)
    {
    }

    public string? Name { get; private set; }
    public string? SomeProperty { get; private set; }

    public IEnumerable<Phrase> Phrases { get; private set; } = Enumerable.Empty<Phrase>();

    protected override void ApplyEvent(IEvent @event)
    {
        switch (@event)
        {
            case ProjectCreatedEvent projectCreatedEvent:
                Apply(projectCreatedEvent);
                break;
            case SomePropertyChangedEvent somePropertyChangedEvent:
                Apply(somePropertyChangedEvent);
                break;
            case PhraseCreatedEvent phraseCreatedEvent:
                Apply(phraseCreatedEvent);
                break;
            case TranslationUpdatedEvent translationUpdatedEvent:
                Apply(translationUpdatedEvent);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static Project Create(IEvent initialEvent)
    {
        var project = new Project(Guid.NewGuid(), Enumerable.Empty<IEvent>());
        project.ApplyAndStoreEvent(initialEvent);
        return project;
    }

    public void ChangeSomeProperty(string newValue)
    {
        ApplyAndStoreEvent(new SomePropertyChangedEvent(new Guid(), newValue));
    }

    public void CreatePhrase(string name)
    {
        ApplyAndStoreEvent(new PhraseCreatedEvent(Guid.NewGuid(), name));
    }

    public void UpdateTranslation(string phraseKey, string locale, string value)
    {
        ApplyAndStoreEvent(new TranslationUpdatedEvent(Guid.NewGuid(), phraseKey, locale, value));
    }

    private void Apply(ProjectCreatedEvent projectCreatedEvent)
    {
        Name = projectCreatedEvent.Name;
    }

    private void Apply(SomePropertyChangedEvent somePropertyChangedEventEvent)
    {
        SomeProperty = somePropertyChangedEventEvent.NewValue;
    }

    private void Apply(PhraseCreatedEvent phraseCreatedEvent)
    {
        Phrases = Phrases.Union(new List<Phrase> { Phrase.Create(phraseCreatedEvent.Name) });
    }

    private void Apply(TranslationUpdatedEvent @event)
    {
        var oldPhrase = Phrases.FirstOrDefault(p => p.Key == @event.PhraseKey);
        if (oldPhrase is null)
            throw new InvariantException("Phrase not found");

        var translation = Translation.Create(@event.Locale, @event.Value);
        var newPhrase = oldPhrase.With(translation);

        Phrases = Phrases.Except(new List<Phrase> { oldPhrase }).Union(new List<Phrase> { newPhrase });
    }
}
