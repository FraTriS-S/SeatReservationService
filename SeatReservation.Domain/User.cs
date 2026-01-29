namespace SeatReservation.Domain;

public class User
{
    public User()
    {
    }

    public Deatails Details { get; set; }

    public Guid Id { get; set; }
}

public record Deatails
{
    public Deatails()
    {
    }

    public string Description { get; set; }

    public string FIO { get; set; }

    public IReadOnlyList<SocialNetwork> Socials { get; set; }
}

public record SocialNetwork
{
    public SocialNetwork()
    {
    }

    public string Name { get; init; }

    public string Link { get; init; }
}