using Microsoft.AspNetCore.Mvc;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Domain;
using RoomScheduling.Host.Dtos;

namespace RoomScheduling.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly Func<DefineRoomCommand, Task> _defineRoomCommandHandler;
    private readonly Func<BookRoomCommand, Task> _bookRoomCommandHandler;
    private readonly Func<string, Task<Room>> _readRoomQueryHandler;
    private readonly Func<Task<IReadOnlyCollection<Room>>> _readRoomsQueryHandler;
    private readonly Func<FindBookingsQuery, Task<FindBookingsQueryResult>> _findBookingsQueryHandler;

    public RoomsController(Func<DefineRoomCommand, Task> defineRoomCommandHandler, Func<BookRoomCommand, Task> bookRoomCommandHandler,
        Func<string, Task<Room>> readRoomQueryHandler, Func<FindBookingsQuery, Task<FindBookingsQueryResult>> findBookingsQueryHandler,
        Func<Task<IReadOnlyCollection<Room>>>  readRoomsQueryHandler)
    {
        _defineRoomCommandHandler = defineRoomCommandHandler;
        _bookRoomCommandHandler = bookRoomCommandHandler;
        _readRoomQueryHandler = readRoomQueryHandler;
        _findBookingsQueryHandler = findBookingsQueryHandler;
        _readRoomsQueryHandler = readRoomsQueryHandler;
    }

    [HttpGet("{name}", Name = "ReadRoom")]
    public async Task<IActionResult> Get(string name) => Ok(await _readRoomQueryHandler(name));


    [HttpGet(Name = "ReadRooms")]
    public async Task<IActionResult> GetAll() => Ok(await _readRoomsQueryHandler());

    [HttpPost(Name = "DefineRoom")]
    public async Task<IActionResult> Post(RoomDto dto)
    {
        if (dto.Name == null)
            return BadRequest("Name cannot be null");
        await _defineRoomCommandHandler(new DefineRoomCommand(dto.NumberOfSeats, dto.HasProjector, dto.HasSoundSystem, dto.HasAirConditioner, dto.Name));
        return CreatedAtAction(nameof(Get), new { name = dto.Name }, null);
    }

    [HttpPost("{name}/bookings",Name = "BookRoom")]
    public async Task<IActionResult> Post(string name, BookRoomDto dto)
    {
        _ = dto.From ?? throw new ArgumentNullException(dto.From);
        _ = dto.To ?? throw new ArgumentNullException(dto.From);
        await _bookRoomCommandHandler(new BookRoomCommand(DateOnly.FromDateTime(dto.Date), name, TimeOnly.Parse(dto.From), TimeOnly.Parse(dto.To)));
        return CreatedAtAction(nameof(GetBookings), new { name = name, date = dto.Date }, null);
    }

    [HttpGet("{name}/bookings", Name = "GetRoomBookings")]
    public async Task<IActionResult> GetBookings(string name, [FromQuery] DateTime date)
    {
        var result = await _findBookingsQueryHandler(new FindBookingsQuery(name, DateOnly.FromDateTime(date)));
        return Ok(new BookingsDto()
        {
            Date = new DateTime(result.Date.Year, result.Date.Month, result.Date.Day),
            Name = name,
            Bookings = result.Bookings.Select(x => new Slot()
            {
                From = x.from.ToShortTimeString(),
                To = x.to.ToShortTimeString()
            }).ToList()
        });
    }
}