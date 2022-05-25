namespace RoomScheduling.Host.Dtos;

public class BookRoomDto
{
    public DateTime Date { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
}