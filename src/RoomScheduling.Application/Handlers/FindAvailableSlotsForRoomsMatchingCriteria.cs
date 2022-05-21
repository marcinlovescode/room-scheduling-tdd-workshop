using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;

namespace RoomScheduling.Application.Handlers;

public class FindAvailableSlotsForRoomsMatchingCriteriaQuery
{
    public int RequiredNumberOfSeats { get; }
    public bool RequiresProjector { get; }
    public bool RequiresSoundSystem { get; }
    public bool RequiresAirConditioner { get; }
    public DateOnly Date { get; }

    public FindAvailableSlotsForRoomsMatchingCriteriaQuery(int requiredNumberOfSeats, bool requiresProjector, bool requiresSoundSystem, bool requiresAirConditioner, DateOnly date)
    {
        RequiredNumberOfSeats = requiredNumberOfSeats;
        RequiresProjector = requiresProjector;
        RequiresSoundSystem = requiresSoundSystem;
        RequiresAirConditioner = requiresAirConditioner;
        Date = date;
    }
}

public class FindAvailableSlotsForRoomsMatchingCriteriaQueryResult
{
    public string Name { get; }
    public int NumberOfSeats { get; }
    public bool HasProjector { get; }
    public bool HasSoundSystem { get; }
    public bool HasAirConditioner { get; }
    public DateOnly Date { get; }
    public IReadOnlyCollection<(TimeOnly from, TimeOnly to)> AvailableSlots { get; }

    public FindAvailableSlotsForRoomsMatchingCriteriaQueryResult(string name, int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner, DateOnly date,
        IReadOnlyCollection<(TimeOnly from, TimeOnly to)> availableSlots)
    {
        Name = name;
        NumberOfSeats = numberOfSeats;
        HasProjector = hasProjector;
        HasSoundSystem = hasSoundSystem;
        HasAirConditioner = hasAirConditioner;
        Date = date;
        AvailableSlots = availableSlots;
    }
}

public class FindAvailableSlotsForRoomsMatchingCriteria
{
    private readonly IRoomDao _roomDao;
    private readonly IDailyScheduleDao _dailyScheduleDao;

    public FindAvailableSlotsForRoomsMatchingCriteria(IRoomDao roomDao, IDailyScheduleDao dailyScheduleDao)
    {
        _roomDao = roomDao;
        _dailyScheduleDao = dailyScheduleDao;
    }

    public async Task<IReadOnlyCollection<FindAvailableSlotsForRoomsMatchingCriteriaQueryResult>> Handle(FindAvailableSlotsForRoomsMatchingCriteriaQuery query)
    {
        var roomsMatchingCriteria = await _roomDao.Find(query.RequiredNumberOfSeats, query.RequiresProjector, query.RequiresSoundSystem, query.RequiresAirConditioner);
        var availableSlots = await _dailyScheduleDao.Get(roomsMatchingCriteria.Select(x => x.Name).ToArray(), query.Date);
        return roomsMatchingCriteria.Select(room => MapRoomAndDailySchedule(room, availableSlots.Single(schedule => schedule.ResourceId == room.Name))).ToList();

    }

    private static FindAvailableSlotsForRoomsMatchingCriteriaQueryResult MapRoomAndDailySchedule(Room room, DailySchedule dailySchedule)
    {
        return new FindAvailableSlotsForRoomsMatchingCriteriaQueryResult(room.Name, room.NumberOfSeats, room.HasProjector, room.HasSoundSystem, room.HasAirConditioner,
            dailySchedule.Date, dailySchedule.AvailableTimeSlots());
    }
}