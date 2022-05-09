using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;

namespace RoomScheduling.Application.Handlers;

public class ReadRoomHandler
{
    private readonly IRoomDao _roomDao;

    public ReadRoomHandler(IRoomDao roomDao)
    {
        _roomDao = roomDao;
    }

    public Task<Room> Handle(string query) => _roomDao.Get(query);
}