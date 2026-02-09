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

    [HttpGet("{eventId:guid}/dapper")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid eventId,
        [FromServices] GetEventByIdDapperQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var request = new GetEventByIdQuery(eventId);
        var result = await handler.Handle(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(
        [FromQuery] GetEventsQuery query,
        [FromServices] GetEventsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("dapper")]
    public async Task<IActionResult> GetById(
        [FromQuery] GetEventsQuery query,
        [FromServices] GetEventsDapperQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(query, cancellationToken);
        return Ok(result);
    }
}