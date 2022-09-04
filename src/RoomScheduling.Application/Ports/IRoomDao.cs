using RoomScheduling.Domain;

namespace RoomScheduling.Application.Ports;

public interface IRoomDao
{
    Task Save(Room room);
    Task<Room> Get(string name);
    Task<IReadOnlyCollection<Room>> GetAll();
    Task<IReadOnlyCollection<Room>> Find(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner);
}