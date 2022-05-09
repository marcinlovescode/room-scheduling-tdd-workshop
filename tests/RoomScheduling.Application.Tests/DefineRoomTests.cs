using System;
using System.Threading.Tasks;
using FluentAssertions;
using RoomScheduling.SqlitePersistence;
using Xunit;

namespace RoomScheduling.Application.Tests;

public class DefineRoomTests
{
    [Fact]
    public async Task Defines_and_persists_room()
    {
        //Arrange
        var command = new
        {
            NumberOfSeats = 5,
            HasProjector = false,
            HasSoundSystem = false,
            HasAirConditioner = false,
            Name = Guid.NewGuid().ToString("N")
        };
        RoomDao dao = null;
        //Act
        Action defineRoom = () => throw new NotImplementedException();
        //Assert
        var persistedRoom = await dao.Get(command.Name);
        persistedRoom.Should().NotBeNull();
    }
}
