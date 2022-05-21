namespace RoomScheduling.Host.Dtos;

public class BookingsDto
{
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public IList<Slot>? Bookings { get; set; }
}
