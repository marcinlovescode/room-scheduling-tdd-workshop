using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;

namespace RoomScheduling.Application.Handlers;

public class ReadRoomsHandler
{
    private readonly IRoomDao _roomDao;

    public ReadRoomsHandler(IRoomDao roomDao)
    {
        _roomDao = roomDao;
    }

    public Task<IReadOnlyCollection<Room>> Handle() => _roomDao.GetAll();
}