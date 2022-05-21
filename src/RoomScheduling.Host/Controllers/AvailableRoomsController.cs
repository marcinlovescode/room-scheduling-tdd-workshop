using Microsoft.AspNetCore.Mvc;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Host.Dtos;


namespace RoomScheduling.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailableRoomsController : ControllerBase
{
    private readonly Func<FindAvailableSlotsForRoomsMatchingCriteriaQuery, Task<IReadOnlyCollection<FindAvailableSlotsForRoomsMatchingCriteriaQueryResult>>> _readRoomQueryHandler;

    public AvailableRoomsController(
        Func<FindAvailableSlotsForRoomsMatchingCriteriaQuery, Task<IReadOnlyCollection<FindAvailableSlotsForRoomsMatchingCriteriaQueryResult>>> readRoomQueryHandler)
    {
        _readRoomQueryHandler = readRoomQueryHandler;
    }

    [HttpGet(Name = "ReadAvailableRooms")]
    public async Task<IActionResult> Get([FromQuery] FindAvailableSlotsForRoomsMatchingCriteriaQueryDto query)
    {
        var queryResult = await _readRoomQueryHandler(new
            FindAvailableSlotsForRoomsMatchingCriteriaQuery(query.RequiredNumberOfSeats, query.RequiresProjector, query.RequiresSoundSystem, query.RequiresAirConditioner, DateOnly
                .FromDateTime(query.Date)));
        return Ok(queryResult.Select(queryResultItem => new AvailableRoomsDto()
        {
            Date = new DateTime(queryResultItem.Date.Year, queryResultItem.Date.Month, queryResultItem.Date.Day, 0, 0, 0, DateTimeKind.Utc),
            Name = queryResultItem.Name,
            AvailableSlots = queryResultItem.AvailableSlots.Select(slot => new Slot()
            {
                From = slot.from.ToShortTimeString(),
                To = slot.to.ToShortTimeString()
            }).ToList(),
            HasProjector = queryResultItem.HasProjector,
            HasAirConditioner = queryResultItem.HasAirConditioner,
            HasSoundSystem = queryResultItem.HasSoundSystem,
            NumberOfSeats = queryResultItem.NumberOfSeats
        }).ToList());
    }
}