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
    
    [Fact]
    public async Task Finds_rooms_by_criteria()
    {
        //Arrange
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        var dbBootstrapper = new Bootstrapper(createDbConnection);
        await dbBootstrapper.Bootstrap();
        var room1 = new Room(10, true, false, false, $"{Guid.NewGuid():N}");
        var room2 = new Room(10, false, true, false, $"{Guid.NewGuid():N}");
        var room3 = new Room(15, true, true, true, $"{Guid.NewGuid():N}");
        var dao = new RoomDao(createDbConnection);
        await dao.Save(room1);
        await dao.Save(room2);
        await dao.Save(room3);
        //Act
        var roomsMatchingCriteria = await dao.Find(numberOfSeats: 10, hasProjector: false, hasSoundSystem: true, hasAirConditioner:false);
        //Assert
        roomsMatchingCriteria.Should().NotContain(x => x.Name == room1.Name);
        roomsMatchingCriteria.Should().Contain(x => x.Name == room2.Name);
        roomsMatchingCriteria.Should().Contain(x => x.Name == room3.Name);
    }

    [Fact]
    public async Task Get_all_rooms()
    {
        //Arrange
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        var dbBootstrapper = new Bootstrapper(createDbConnection);
        await dbBootstrapper.Bootstrap();
        var room1 = new Room(10, true, false, false, $"{Guid.NewGuid():N}");
        var room2 = new Room(15, false, true, false, $"{Guid.NewGuid():N}");
        var dao = new RoomDao(createDbConnection);
        await dao.Save(room1);
        await dao.Save(room2);
        //Act
        var roomsMatchingCriteria = await dao.GetAll();
        //Assert
        roomsMatchingCriteria.Should().Contain(x => x.Name == room1.Name);
        roomsMatchingCriteria.Should().Contain(x => x.Name == room2.Name);
    }
}