using System;
using System.Threading.Tasks;
using FluentAssertions;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Domain;
using RoomScheduling.Fixtures;
using RoomScheduling.SqlitePersistence;
using Xunit;

namespace RoomScheduling.Application.Tests;

public class RoomHandlersTests
{
    [Fact]
    public async Task Defines_and_persists_room()
    {
        //Arrange
        var command = new DefineRoomCommand(5, false, false, false, Guid.NewGuid().ToString("N"));
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        await new Bootstrapper(createDbConnection).Bootstrap();
        var dao = new RoomDao(createDbConnection);
        var commandHandler = new DefineRoomHandler(dao);
        //Act
        await commandHandler.Handle(command);
        //Assert
        var persistedRoom = await dao.Get(command.Name);
        persistedRoom.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Returns_room()
    {
        //Arrange
        var query = Guid.NewGuid().ToString("N");
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        await new Bootstrapper(createDbConnection).Bootstrap();
        var dao = new RoomDao(createDbConnection);
        await dao.Save(new Room(10, false, false, false, query));
        var queryHandler = new ReadRoomHandler(dao);
        //Act
        var result = await queryHandler.Handle(query);
        //Assert
        result.Should().NotBeNull();
    }
}