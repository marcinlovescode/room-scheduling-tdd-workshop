namespace RoomScheduling.Domain;

public class Room
{
    public int NumberOfSeats { get; }
    public bool HasProjector { get; }
    public bool HasSoundSystem { get; }
    public bool HasAirConditioner { get; }
    public string Name { get; }

    private Room(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner, string name)
    {
        NumberOfSeats = numberOfSeats;
        HasProjector = hasProjector;
        HasSoundSystem = hasSoundSystem;
        HasAirConditioner = hasAirConditioner;
        Name = name;
    }

    public static Room Define(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner, string name)
    {
        if (name.Length < 3)
            throw new ArgumentException("Room name length must be greater or equal 3");
        return new Room(numberOfSeats, hasProjector, hasSoundSystem, hasAirConditioner, name);
    }
}