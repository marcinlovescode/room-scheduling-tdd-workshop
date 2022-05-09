using System;
using System.Threading.Tasks;
using FluentAssertions;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Fixtures;
using RoomScheduling.SqlitePersistence;
using Xunit;

namespace RoomScheduling.Application.Tests;

public class DefineRoomTests
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
}