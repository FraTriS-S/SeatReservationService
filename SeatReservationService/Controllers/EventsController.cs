using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.Events.Queries;
using SeatReservation.Contracts.Events;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid eventId,
        [FromServices] GetEventByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var request = new GetEventByIdQuery(eventId);
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result);
    }
}