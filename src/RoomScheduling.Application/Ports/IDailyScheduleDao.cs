using RoomScheduling.Domain;

namespace RoomScheduling.Application.Ports;

public interface IDailyScheduleDao
{
    Task<IReadOnlyCollection<DailySchedule>> Get(string[] resources, DateOnly date);
}