using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using SeatReservation.Domain.Events.ValueObjects;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Domain.Events;

public record EventId(Guid Value);

public class Event
{
    public Event(
        EventId id,
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        EventDetails eventDetails,
        EventType type,
        IEventInfo info)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        StartDate = startDate;
        EndDate = endDate;
        Status = EventStatus.Planned;
        Details = eventDetails;
        Type = type;
        Info = info;
    }

    public EventId Id { get; } = null!;
    public VenueId VenueId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime EventDate { get; private set; }
    public EventDetails Details { get; private set; } = null!;
    public EventType Type { get; private set; }
    public IEventInfo Info { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public EventStatus Status { get; private set; }

    public bool IsAvailableForReservation() => Status == EventStatus.Planned && StartDate > DateTime.UtcNow;

    private static Result<EventDetails, Error> Validate(
        string name, DateTime eventDate, DateTime startDate, DateTime endDate, int capacity, string description)
    {
        if (startDate >= endDate || startDate <= DateTime.UtcNow || endDate <= DateTime.UtcNow)
        {
            return Error.Validation("event.time", "The time of the event is indicated incorrectly");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("event.name", "Event name is required");
        }

        if (eventDate < DateTime.UtcNow)
        {
            return Error.Validation("event.date", "Event date can`t be in the past");
        }

        if (capacity < 0)
        {
            return Error.Validation("event.capacity", "Event capacity must be greater then zero");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.Validation("event.description", "Event description is required");
        }

        return new EventDetails(capacity, description);
    }

    public static Result<Event, Error> CreateConcert(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string performer)
    {
        var detailsResult = Validate(name, eventDate, startDate, endDate, capacity, description);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrWhiteSpace(performer))
        {
            return Error.Validation("event.performer", "Event performer is required");
        }

        var concertInfo = new ConcertInfo(performer);
        var eventId = new EventId(Guid.NewGuid());

        return new Event(eventId, venueId, name, eventDate, startDate, endDate, detailsResult.Value, EventType.Concert, concertInfo);
    }

    public static Result<Event, Error> CreateConference(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string speaker,
        string topic)
    {
        var detailsResult = Validate(name, eventDate, startDate, endDate, capacity, description);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            return Error.Validation("event.speaker", "Event speaker is required");
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            return Error.Validation("event.topic", "Event topic is required");
        }

        var conferenceInfo = new ConferenceInfo(speaker, topic);
        var eventId = new EventId(Guid.NewGuid());

        return new Event(eventId, venueId, name, eventDate, startDate, endDate, detailsResult.Value, EventType.Conference, conferenceInfo);
    }

    public static Result<Event, Error> CreateOnline(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string url)
    {
        var detailsResult = Validate(name, eventDate, startDate, endDate, capacity, description);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return Error.Validation("event.url", "Event url is required");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Error.Validation("event.url", "Event url is not valid");
        }

        var onlineInfo = new OnlineInfo(url);
        var eventId = new EventId(Guid.NewGuid());

        return new Event(eventId, venueId, name, eventDate, startDate, endDate, detailsResult.Value, EventType.Online, onlineInfo);
    }

    [UsedImplicitly]
    private Event()
    {
        // EF Core
    }
}