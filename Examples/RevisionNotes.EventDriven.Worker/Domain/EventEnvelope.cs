namespace RevisionNotes.EventDriven.Worker.Domain;

public sealed record EventEnvelope(
    string EventId,
    string EventType,
    string AggregateId,
    DateTimeOffset CreatedAtUtc,
    int Attempt,
    string Payload);