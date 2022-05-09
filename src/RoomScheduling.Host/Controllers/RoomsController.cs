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
    private readonly Func<string, Task<Room>> _readRoomQueryHandler;

    public RoomsController(Func<DefineRoomCommand, Task> defineRoomCommandHandler, Func<string, Task<Room>> readRoomQueryHandler)
    {
        _defineRoomCommandHandler = defineRoomCommandHandler;
        _readRoomQueryHandler = readRoomQueryHandler;
    }
    
    [HttpGet(Name = "ReadRoom")]
    public async Task<IActionResult> Get(string name) => Ok(await _readRoomQueryHandler(name));

    [HttpPost(Name = "DefineRoom")]
    public async Task<IActionResult> Post(RoomDto dto)
    {
        if (dto.Name == null) 
            return BadRequest("Name cannot be null");
        await _defineRoomCommandHandler(new DefineRoomCommand(dto.NumberOfSeats, dto.HasProjector, dto.HasSoundSystem, dto.HasAirConditioner, dto.Name));
        return CreatedAtAction(nameof(Get), new{name=dto.Name}, null);
    }
}