namespace SeatReservation.Domain.Events;

public class Event
{
    public Event(Guid id, Guid venueId, string name, DateTime eventDate, EventDetails eventDetails)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        Details = eventDetails;
    }

    public Guid Id { get; }
    public Guid VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }
    public EventDetails Details { get; private set; }
}