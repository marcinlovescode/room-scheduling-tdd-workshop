using RoomScheduling.Domain;

namespace RoomScheduling.Application.Ports;

public interface IDailyScheduleDao
{
    Task<IReadOnlyCollection<DailySchedule>> Get(string[] resources, DateOnly date);
    Task<DailySchedule> Get(string resourceId, DateOnly date);
    Task Save(DailySchedule dailySchedule);
}