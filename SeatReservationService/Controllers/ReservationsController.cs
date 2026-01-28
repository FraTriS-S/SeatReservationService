using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.Reservations;
using SeatReservation.Contracts;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateReservation(
        [FromServices] CreateReservationHandler handler,
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("adjacent")]
    public async Task<IActionResult> ReserveAdjacentSeats(
        [FromServices] ReserveAdjacentSeatsHandler handler,
        [FromBody] ReserveAdjacentSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}