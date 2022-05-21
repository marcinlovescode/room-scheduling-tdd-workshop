namespace RoomScheduling.Host.Dtos;

public class AvailableRoomsDto
{
    public string? Name { get; set; }
    public int NumberOfSeats { get; set; }
    public bool HasProjector { get; set; }
    public bool HasSoundSystem { get; set; }
    public bool HasAirConditioner { get; set; }
    public DateTime Date { get; set; }
    public IList<Slot>? AvailableSlots { get; set; }
}

public class Slot
{
    public string From { get; set; }
    public string To { get; set; }
}