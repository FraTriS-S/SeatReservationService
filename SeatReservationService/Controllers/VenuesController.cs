using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.Venues;
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

    [HttpPatch("/name")]
    public async Task<IActionResult> Update(
        [FromServices] UpdateVenueNameHandler handler,
        [FromBody] UpdateVenueNameRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPatch("/name/by-prefix")]
    public async Task<IActionResult> UpdateByPrefix(
        [FromServices] UpdateVenueNameByPrefixHandler handler,
        [FromBody] UpdateVenueNameByPrefixRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result.IsSuccess);
    }

    [HttpPatch("/seats")]
    public async Task<IActionResult> UpdateByPrefix(
        [FromServices] UpdateVenueSeatsHandler handler,
        [FromBody] UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result.IsSuccess);
    }
}