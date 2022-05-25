using Microsoft.AspNetCore.Mvc;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Domain;
using RoomScheduling.Host.Dtos;

namespace RoomScheduling.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly Func<SendNotificationAboutBookingsCommand, Task> _commandHandler;

    public NotificationsController(Func<SendNotificationAboutBookingsCommand, Task> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [HttpPost("bookings/rooms/{name}",Name = "SendEmailNotificationAboutBookings")]
    public async Task<IActionResult> Post(string name, SendNotificationAboutBookingsDto dto)
    {
        _ = dto.Email ?? throw new ArgumentNullException(dto.Email);
        await _commandHandler(new SendNotificationAboutBookingsCommand(name, DateOnly.FromDateTime(dto.Date), dto.Email));
        return Accepted();
    }
}