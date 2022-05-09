namespace RoomScheduling.Domain;

public class DailySchedule
{
    public string ResourceId { get; }
    public DateOnly Date { get; }
    public IReadOnlyCollection<(TimeOnly from, TimeOnly to)> Bookings => _bookings.AsReadOnly();

    private readonly List<(TimeOnly from, TimeOnly to)> _bookings;

    public DailySchedule(string resourceId, DateOnly date)
    {
        ResourceId = resourceId;
        Date = date;
        _bookings = new List<(TimeOnly from, TimeOnly to)>();
    }

    public void Book(TimeOnly from, TimeOnly to)
    {
        _bookings.Add((from, to));
    }

    public bool IsTimeSlotAvailable(TimeOnly from, TimeOnly to) => !_bookings.Contains((from, to));
}