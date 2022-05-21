namespace RoomScheduling.Host.Dtos;

public class AvailableRoomsDto
{
    public string? Name { get; set; }
    public int NumberOfSeats { get; set; }
    public bool HasProjector { get; set; }
    public bool HasSoundSystem { get; set; }
    public bool HasAirConditioner { get; set; }
    public DateOnly Date { get; set; }
    public IReadOnlyCollection<(TimeOnly from, TimeOnly to)>? AvailableSlots { get; set; }
}