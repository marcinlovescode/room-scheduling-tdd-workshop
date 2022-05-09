using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using RoomScheduling.Domain;
using RoomScheduling.Fixtures;

namespace RoomScheduling.SqlitePersistence.Tests;

public class RoomDaoTests
{
    [Fact]
    public async Task Saves_room()
    {
        //Arrange
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        var dbBootstrapper = new Bootstrapper(createDbConnection);
        await dbBootstrapper.Bootstrap();
        var room = new Room(10, true, true, true, $"{Guid.NewGuid():N}");
        var dao = new RoomDao(createDbConnection);
        //Act
        await dao.Save(room);
        //Assert
        var roomFromDb = await dao.Get(room.Name);
        roomFromDb.HasProjector.Should().Be(room.HasProjector);
        roomFromDb.HasAirConditioner.Should().Be(room.HasAirConditioner);
        roomFromDb.HasSoundSystem.Should().Be(room.HasSoundSystem);
        roomFromDb.NumberOfSeats.Should().Be(room.NumberOfSeats);
    }
}