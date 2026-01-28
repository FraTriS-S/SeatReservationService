using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Events;

public static class AdjacentSeatsFinder
{
    public static List<Seat> FindAdjacentSeatsInPreferredRow(IReadOnlyList<Seat> availableSeats, int requiredCount, int preferredRowNumber)
    {
        if (requiredCount <= 0)
        {
            return [];
        }

        var seatsInRow = availableSeats.Where(s => s.RowNumber == preferredRowNumber)
            .OrderBy(x => x.SeatNumber)
            .ToList();

        return FindAdjacentSeatsInRow(seatsInRow, requiredCount);
    }

    public static List<Seat> FindBestAdjacentSeats(IReadOnlyList<Seat> availableSeats, int requiredCount)
    {
        if (requiredCount <= 0 || availableSeats.Count < requiredCount)
        {
            return [];
        }

        var groupedByRow = availableSeats.GroupBy(s => s.RowNumber);

        foreach (var row in groupedByRow.OrderBy(g => g.Key))
        {
            var seatsInRow = row.OrderBy(x => x.SeatNumber).ToList();

            var adjacentSeats = FindAdjacentSeatsInRow(seatsInRow, requiredCount);

            if (adjacentSeats.Count == requiredCount)
            {
                return adjacentSeats;
            }
        }

        return [];
    }

    private static List<Seat> FindAdjacentSeatsInRow(List<Seat> seatsInRow, int requiredCount)
    {
        if (seatsInRow.Count < requiredCount)
        {
            return [];
        }

        for (var i = 0; i <= seatsInRow.Count - requiredCount; i++)
        {
            List<Seat> candidateSeats = [];
            var isAdjacent = true;

            for (var j = 0; j < requiredCount; j++)
            {
                var currentSeat = seatsInRow[i + j];
                candidateSeats.Add(currentSeat);

                if (j <= 0) continue;

                var previousSeat = seatsInRow[i + j - 1];

                if (currentSeat.SeatNumber == previousSeat.SeatNumber + 1) continue;

                isAdjacent = false;
                break;
            }

            if (isAdjacent)
            {
                return candidateSeats;
            }
        }

        return [];
    }
}