using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeatReservation.Domain.Events.ValueObjects;

namespace SeatReservation.Infrastructure.Postgres.Converters;

public class EventInfoValueConverter : ValueConverter<IEventInfo, string>
{
    public EventInfoValueConverter() : base(iValue => InfoToString(iValue),
        str => StringToInfo(str))
    {
    }

    private static string InfoToString(IEventInfo info) => info switch
    {
        ConcertInfo x => $"Concert: {x.Performer}",
        ConferenceInfo x => $"Conference: {x.Speaker} | {x.Topic}",
        OnlineInfo x => $"Online: {x.Url}",
        _ => throw new NotSupportedException("Unsupported event info type")
    };

    private static IEventInfo StringToInfo(string info)
    {
        var split = info.Split(':', 2);
        var type = split[0];
        var data = split[1];

        return type switch
        {
            "Concert" => new ConcertInfo(data),
            "Conference" => new ConferenceInfo(data.Split('|')[0], data.Split('|')[1]),
            "Online" => new OnlineInfo(data),
            _ => throw new NotSupportedException($"Unknown type: {type}")
        };
    }
}