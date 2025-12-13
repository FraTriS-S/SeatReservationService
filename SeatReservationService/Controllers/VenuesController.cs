using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application;
using SeatReservation.Contracts;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVenueHandler handler,
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result.Value);
    }
}