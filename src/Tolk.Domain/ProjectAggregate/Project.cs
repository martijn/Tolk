using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.Domain.ProjectAggregate;

public class Project : Aggregate
{
    public Project(Guid id, IEnumerable<IEvent> events) : base(id, events)
    {
    }

    public bool Archived { get; private set; }
    public string? Name { get; private set; }
    public IEnumerable<Phrase> Phrases { get; private set; } = Enumerable.Empty<Phrase>();

    protected override void ApplyEvent(IEvent @event)
    {
        switch (@event)
        {
            case ProjectArchivedEvent projectArchivedEvent:
                Apply(projectArchivedEvent);
                break;
            case ProjectCreatedEvent projectCreatedEvent:
                Apply(projectCreatedEvent);
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

    public void Archive()
    {
        ApplyAndStoreEvent(new ProjectArchivedEvent(Guid.NewGuid()));
    }
        
    public void CreatePhrase(string name)
    {
        ApplyAndStoreEvent(new PhraseCreatedEvent(Guid.NewGuid(), name));
    }

    public void UpdateTranslation(string phraseKey, string locale, string value)
    {
        ApplyAndStoreEvent(new TranslationUpdatedEvent(Guid.NewGuid(), phraseKey, locale, value));
    }

    private void Apply(ProjectArchivedEvent projectArchivedEvent)
    {
        Archived = true;
    }
    
    private void Apply(ProjectCreatedEvent projectCreatedEvent)
    {
        Name = projectCreatedEvent.Name;
    }

    private void Apply(PhraseCreatedEvent phraseCreatedEvent)
    {
        if (Archived) throw new InvariantException("Cannot modify an archived project");
        
        Phrases = Phrases.Union(new List<Phrase> { Phrase.Create(phraseCreatedEvent.Name) });
    }

    private void Apply(TranslationUpdatedEvent @event)
    {
        if (Archived) throw new InvariantException("Cannot modify an archived project");
        
        var oldPhrase = Phrases.FirstOrDefault(p => p.Key == @event.PhraseKey);
        if (oldPhrase is null)
            throw new InvariantException("Phrase not found");

        var translation = Translation.Create(@event.Locale, @event.Value);
        var newPhrase = oldPhrase.With(translation);

        Phrases = Phrases.Except(new List<Phrase> { oldPhrase }).Union(new List<Phrase> { newPhrase });
    }
}
