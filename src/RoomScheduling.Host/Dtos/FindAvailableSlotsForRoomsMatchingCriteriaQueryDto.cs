namespace RoomScheduling.Host.Dtos;

public class FindAvailableSlotsForRoomsMatchingCriteriaQueryDto
{
    public int RequiredNumberOfSeats { get; set; }
    public bool RequiresProjector { get; set; }
    public bool RequiresSoundSystem { get; set; }
    public bool RequiresAirConditioner { get; set; }
    public DateTime Date { get; set; }
}