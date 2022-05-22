using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Domain;
using RoomScheduling.Fixtures;
using RoomScheduling.SqlitePersistence;
using SendGrid;
using SendGrid.Helpers.Mail;
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

    [Fact]
    public async Task Sends_email_with_bookings_for_room()
    {
        //Arrange
        var command = new  SendNotificationAboutBookingsCommand(Guid.NewGuid().ToString("N"), DateOnly.FromDateTime(DateTime.Now),"test@email.com");
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        await new Bootstrapper(createDbConnection).Bootstrap();
        var roomDao = new RoomDao(createDbConnection);
        var scheduleDao = new DailyScheduleDao(createDbConnection);
        var room = new Room(5, true, true, true, command.Name);
        var schedule = new DailySchedule(room.Name, command.Date,
            new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(10, 00), new TimeOnly(13, 00)), (new TimeOnly(14, 00), new TimeOnly(15, 00))});
        await roomDao.Save(room);
        await scheduleDao.Save(schedule);
        var sendGridResponse = new Response(HttpStatusCode.Accepted, new StringContent("OK"), null);
        var sendGridMock = new Mock<ISendGridClient>();
        SendGridMessage sendGridMessageToAssert = null;
        sendGridMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .Callback((SendGridMessage message, CancellationToken _) => { sendGridMessageToAssert = message; })
            .Returns(Task.FromResult(sendGridResponse));
        var commandHandler = new SendNotificationAboutBookingsHandler(sendGridMock.Object, scheduleDao);
        //Act
        await commandHandler.Handle(command);
        //Assert
        if (sendGridMessageToAssert == null)
            throw new Exception("SendGrid client not called");
        var emails = sendGridMessageToAssert.Personalizations.SelectMany(x => x.Tos).Select(x => x.Email).ToList();
        emails[0].Should().Be(command.Email);
        sendGridMessageToAssert.HtmlContent.Should().Be("Bookings: 10:00-13:00, 14:00-15:00");
    }
}