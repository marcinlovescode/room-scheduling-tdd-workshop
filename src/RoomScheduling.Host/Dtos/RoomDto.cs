namespace RoomScheduling.Host.Dtos;

public class RoomDto
{
    public int NumberOfSeats { get; set; }
    public bool HasProjector { get; set; }
    public bool HasSoundSystem { get; set; }
    public bool HasAirConditioner { get; set; }
    public string? Name { get; set; }
}