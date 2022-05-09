using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;

namespace RoomScheduling.Application.Handlers;

public class DefineRoomCommand
{
    public int NumberOfSeats { get; }
    public bool HasProjector { get; }
    public bool HasSoundSystem { get; }
    public bool HasAirConditioner { get; }
    public string Name { get; }

    public DefineRoomCommand(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner, string name)
    {
        NumberOfSeats = numberOfSeats;
        HasProjector = hasProjector;
        HasSoundSystem = hasSoundSystem;
        HasAirConditioner = hasAirConditioner;
        Name = name;
    }
}

public class DefineRoomHandler
{
    private readonly IRoomDao _roomDao;

    public DefineRoomHandler(IRoomDao roomDao)
    {
        _roomDao = roomDao;
    }

    public async Task Handle(DefineRoomCommand command)
    {
        var room = Room.Define(command.NumberOfSeats, command.HasProjector, command.HasSoundSystem, command.HasAirConditioner, command.Name);
        await _roomDao.Save(room);
    }
}