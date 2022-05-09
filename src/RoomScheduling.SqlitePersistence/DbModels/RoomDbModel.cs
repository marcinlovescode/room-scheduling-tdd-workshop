using RoomScheduling.Domain;

namespace RoomScheduling.SqlitePersistence.DbModels;

public class RoomDbModel
{
    public string Name { get; set; }
    public int NumberOfSeats { get; set; }
    public int HasProjector { get; set; }
    public int HasSoundSystem { get; set; }
    public int HasAirConditioner { get; set; }

    public RoomDbModel(long numberOfSeats, long hasProjector, long hasSoundSystem, long hasAirConditioner, string name)
    {
        Name = name;
        NumberOfSeats = Convert.ToInt32(numberOfSeats);
        HasProjector = Convert.ToInt32(hasProjector);
        HasSoundSystem = Convert.ToInt32(hasSoundSystem);
        HasAirConditioner = Convert.ToInt32(hasAirConditioner);
    }
    
    public static RoomDbModel FromDomain(Room room) =>
        new(
            room.NumberOfSeats,
            Convert.ToInt32(room.HasProjector),
            Convert.ToInt32(room.HasSoundSystem),
            Convert.ToInt32(room.HasAirConditioner),
            room.Name
        );


    public Room ToDomain() =>
        new(NumberOfSeats,
            Convert.ToBoolean(HasProjector),
            Convert.ToBoolean(HasSoundSystem),
            Convert.ToBoolean(HasAirConditioner),
            Name);
}