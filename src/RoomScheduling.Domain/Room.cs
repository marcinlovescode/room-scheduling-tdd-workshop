namespace RoomScheduling.Domain;

public class Room
{
    public int NumberOfSeats { get; }
    public bool HasProjector { get; }
    public bool HasSoundSystem { get; }
    public bool HasAirConditioner { get; }

    private Room(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner)
    {
        NumberOfSeats = numberOfSeats;
        HasProjector = hasProjector;
        HasSoundSystem = hasSoundSystem;
        HasAirConditioner = hasAirConditioner;
    }

    public static Room Define(int numberOfSeats, bool hasProjector, bool hasSoundSystem, bool hasAirConditioner) =>
        new(numberOfSeats, hasProjector, hasSoundSystem, hasAirConditioner);
}