namespace EventSourcing.Events;

/// <summary>
/// Base event type
/// </summary>
/// <param name="StreamId">Aggregate identifier</param>
public abstract record Event(Guid StreamId)
{
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}

