using EventSourcing.Events;

namespace EventSourcing.BankAccounts;

public abstract class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    private readonly List<Event> _events = [];
    public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

    protected void EventGenerated(Event @event)
    {
        _events.Add(@event);
    }

    protected void RemoveEvent(Event @event)
    {
        _events.Remove(@event);
    }
    protected void Clear()
    {
        _events.Clear();
    }

}
