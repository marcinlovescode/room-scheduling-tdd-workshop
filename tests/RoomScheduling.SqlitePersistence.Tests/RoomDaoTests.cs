using System;
using System.Threading.Tasks;
using Xunit;

namespace RoomScheduling.SqlitePersistence.Tests;

public class RoomDaoTests
{
    [Fact]
    public async Task Saves_room()
    {
        //Arrange
        var room = RoomScheduling.Domain.Room.Define(10, true, true, true, $"{Guid.NewGuid():N}");
        //Act
        await RoomDao.Save(room);
        //Assert
        var roomFromDb = await RoomDao.Get(room.Name);
        roomFromDb.HasProjector.Should().Equal(room.HasProjector);
        roomFromDb.HasAirConditioner.Should().Equal(room.HasAirConditioner);
        roomFromDb.HasSoundSystem.Should().Equal(room.HasSoundSystem);
        roomFromDb.NumberOfSeats.Should().Equal(room.NumberOfSeats);
    }
}