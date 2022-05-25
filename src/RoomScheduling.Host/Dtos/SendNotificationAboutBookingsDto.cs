namespace RoomScheduling.Host.Dtos;

public class SendNotificationAboutBookingsDto
{
    public DateTime Date { get; set; }
    public string? Email { get; set; }
}