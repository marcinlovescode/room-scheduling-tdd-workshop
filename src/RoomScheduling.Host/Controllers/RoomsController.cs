using Microsoft.AspNetCore.Mvc;
using RoomScheduling.Host.Dtos;

namespace RoomScheduling.Host.Controllers;

[ApiController]
[Route("api/[controller]")]

public class RoomsController : ControllerBase
{
    [HttpPost(Name = "DefineRoom")]
    public IActionResult Post(RoomDto dto)
    {
        throw new NotImplementedException();
    }
}