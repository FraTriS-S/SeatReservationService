using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Domain.Venues;

public record VenueName
{
    private VenueName(string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
    }

    public string Prefix { get; }
    public string Name { get; }

    public static Result<VenueName, Error> Create(string prefix, string name)
    {
        if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("venue.name", "Venue name can`t be empty or whitespace");
        }

        if (prefix.Length > LengthConstants.LENGTH_50 || name.Length > LengthConstants.LENGTH_500)
        {
            return Error.Validation("venue.name", "Venue name is too long");
        }

        return new VenueName(prefix, name);
    }

    public override string ToString() => $"{Prefix}-{Name}";
}