using Xunit;
using FluentAssertions;

namespace RoomScheduling.Domain.Tests;

public class RoomTests
{
    [Fact]
    public void Define_room_creates_fully_configured_room()
    {
        //Arrange
        var numberOfSeats = 10;
        var hasProjector = true;
        var hasSoundSystem = true;
        var hasAirConditioner = false;
        //Act
        var definedRoom = Room.Define(numberOfSeats,hasProjector,hasSoundSystem,hasAirConditioner);
        //Assert
        definedRoom.NumberOfSeats.Should().Be(numberOfSeats);
        definedRoom.HasProjector.Should().Be(hasProjector);
        definedRoom.HasSoundSystem.Should().Be(hasSoundSystem);
        definedRoom.HasAirConditioner.Should().Be(hasAirConditioner);
    }
}