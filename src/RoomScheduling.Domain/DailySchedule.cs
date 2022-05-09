namespace RoomScheduling.Domain;

public class DailySchedule
{
    public string ResourceId { get; }
    public DateOnly Date { get; }

    public DailySchedule(string resourceId, DateOnly date)
    {
        ResourceId = resourceId;
        Date = date;
    }
}