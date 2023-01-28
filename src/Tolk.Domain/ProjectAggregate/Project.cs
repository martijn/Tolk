using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.Domain.ProjectAggregate;

public class Project : AggregateBase, IAggregate
{
    public Project(Guid id, IEnumerable<IEvent> events) : base(id, events)
    {
    }

    public string? Name { get; private set; }
    public string? SomeProperty { get; private set; }

    public IEnumerable<Phrase> Phrases { get; private set; } = Enumerable.Empty<Phrase>();

    public override void ApplyEvent(IEvent @event)
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
        };

        Version += 1;
        
        if (!_isReplaying)
        {
            @event.Version = Version;
            @event.Aggregate = $"{GetType().Name}-{Id}";
            _unsavedEvents.Add(@event);
            
        }
    }

    public static Project Create(IEvent initialEvent)
    {
        var project = new Project(Guid.NewGuid(), Enumerable.Empty<IEvent>());
        project.ApplyEvent(initialEvent);
        return project;
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

    public void Apply(TranslationUpdatedEvent @event)
    {
        var oldPhrase = Phrases.FirstOrDefault(p => p.Key == @event.PhraseKey);
        if (oldPhrase is null)
            throw new InvariantException("Phrase not found");

        var translation = Translation.Create(@event.Locale, @event.Value);
        var newPhrase = oldPhrase.With(translation);

        Phrases = Phrases.Except(new List<Phrase> { oldPhrase }).Union(new List<Phrase> { newPhrase });
    }

    public void ChangeSomeProperty(string newValue)
    {
        ApplyEvent(new SomePropertyChangedEvent(new Guid(), newValue));
    }

    public void CreatePhrase(string name)
    {
        ApplyEvent(new PhraseCreatedEvent(Guid.NewGuid(), name));
    }

    public void UpdateTranslation(string phraseKey, string locale, string value)
    {
        ApplyEvent(new TranslationUpdatedEvent(Guid.NewGuid(), phraseKey, locale, value));
    }
}
