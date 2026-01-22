namespace SeatReservation.Domain.Events;

public enum EventStatus
{
    Unknown = 0,
    Planned = 1,
    InProgress = 2,
    Finished = 3,
    Canceled = 4,
}