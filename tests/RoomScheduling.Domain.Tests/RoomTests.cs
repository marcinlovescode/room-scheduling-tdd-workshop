using System;
using Xunit;
using FluentAssertions;

namespace RoomScheduling.Domain.Tests;

public class RoomTests
{
    private const int ValidNumberOfSeats = 10;
    private const bool ValidHasProjector = true;
    private const bool ValidHasSoundSystem = true;
    private const bool ValidHasAirConditioner = false;
    private readonly string _validRoomNumber = $"{Guid.NewGuid():N}"[..3];
    
    [Fact]
    public void Define_room_creates_fully_configured_room()
    {
        //Act
        var definedRoom = Room.Define(ValidNumberOfSeats, ValidHasProjector, ValidHasSoundSystem, ValidHasAirConditioner, _validRoomNumber);
        //Assert
        definedRoom.NumberOfSeats.Should().Be(ValidNumberOfSeats);
        definedRoom.HasProjector.Should().Be(ValidHasProjector);
        definedRoom.HasSoundSystem.Should().Be(ValidHasSoundSystem);
        definedRoom.HasAirConditioner.Should().Be(ValidHasAirConditioner);
        definedRoom.Name.Should().Be(_validRoomNumber);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Room_name_must_be_longer_or_equal_to_three_characters(int numberOfCharacters)
    {
        //Arrange
        var expectedErrorMessage = "Room name length must be greater or equal 3";
        var roomName = $"{Guid.NewGuid():N}"[..numberOfCharacters];
        //Act
        Action definedRoom = () => Room.Define(ValidNumberOfSeats, ValidHasProjector, ValidHasSoundSystem, ValidHasAirConditioner, roomName);
        //Assert
        definedRoom.Should().Throw<ArgumentException>().WithMessage(expectedErrorMessage);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Number_of_seats_must_be_greater_or_equal_to_one(int numberOfSeats)
    {
        //Arrange
        var expectedErrorMessage = "Number of seats must be greater than or equal 1";
        //Act
        Action definedRoom = () => Room.Define(numberOfSeats, ValidHasProjector, ValidHasSoundSystem, ValidHasAirConditioner, _validRoomNumber);
        //Assert
        definedRoom.Should().Throw<ArgumentException>().WithMessage(expectedErrorMessage);
    }
}