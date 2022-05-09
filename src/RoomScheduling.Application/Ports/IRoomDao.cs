using RoomScheduling.Domain;

namespace RoomScheduling.Application.Ports;

public interface IRoomDao
{
    Task Save(Room room);
    Task<Room> Get(string name);
}