using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Domain;
using RoomScheduling.Fixtures;
using RoomScheduling.SqlitePersistence;
using Xunit;

namespace RoomScheduling.Application.Tests;

public class FindAvailableSlotsForRoomsMatchingCriteriaTests
{
    [Fact]
    public async Task Returns_rooms_and_slots()
    {
        //Arrange
        var query = new FindAvailableSlotsForRoomsMatchingCriteriaQuery(5, false, true, true, DateOnly.FromDateTime(DateTime.Now));
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        await new Bootstrapper(createDbConnection).Bootstrap();
        var roomDao = new RoomDao(createDbConnection);
        var scheduleDao = new DailyScheduleDao(createDbConnection);
        var rooms = new[]
        {
            new Room(5, true, true, true, $"{Guid.NewGuid():N}"),
            new Room(6, true, false, true, $"{Guid.NewGuid():N}"),
            new Room(7, false, false, false, $"{Guid.NewGuid():N}"),
            new Room(8, false, true, true, $"{Guid.NewGuid():N}")
        };
        var schedules = new[]
        {
            new DailySchedule(rooms[0].Name, query.Date,
                new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(10, 00), new TimeOnly(13, 00)), (new TimeOnly(14, 30), new TimeOnly(16, 00)) }),
            new DailySchedule(rooms[1].Name, query.Date,
                new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(11, 00), new TimeOnly(12, 00)), (new TimeOnly(16, 00), new TimeOnly(18, 00)) }),
            new DailySchedule(rooms[2].Name, query.Date, new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(08, 00), new TimeOnly(15, 00)) })
        };
        await Task.WhenAll(rooms.Select(x => roomDao.Save(x)));
        await Task.WhenAll(schedules.Select(x => scheduleDao.Save(x)));

        var queryHandler = new FindAvailableSlotsForRoomsMatchingCriteria(roomDao,scheduleDao);
        //Act
        var result = await queryHandler.Handle(query);
        //Assert
        result.Should().HaveCount(2);
        var fiveSlotsRoom = result.Single(x => x.Name == rooms[0].Name);
        var eightSlotsRoom = result.Single(x => x.Name == rooms[3].Name);
        fiveSlotsRoom.NumberOfSeats.Should().Be(rooms[0].NumberOfSeats);
        fiveSlotsRoom.HasProjector.Should().Be(rooms[0].HasProjector);
        fiveSlotsRoom.HasSoundSystem.Should().Be(rooms[0].HasSoundSystem);
        fiveSlotsRoom.HasAirConditioner.Should().Be(rooms[0].HasAirConditioner);
        fiveSlotsRoom.AvailableSlots.Should().Contain((new TimeOnly(00, 00), new TimeOnly(10, 00)));
        fiveSlotsRoom.AvailableSlots.Should().Contain((new TimeOnly(13, 00), new TimeOnly(14, 30)));
        fiveSlotsRoom.AvailableSlots.Should().Contain((new TimeOnly(16, 00), new TimeOnly(00, 00)));
        eightSlotsRoom.NumberOfSeats.Should().Be(rooms[3].NumberOfSeats);
        eightSlotsRoom.HasProjector.Should().Be(rooms[3].HasProjector);
        eightSlotsRoom.HasSoundSystem.Should().Be(rooms[3].HasSoundSystem);
        eightSlotsRoom.HasAirConditioner.Should().Be(rooms[3].HasAirConditioner);
        eightSlotsRoom.AvailableSlots.Should().Contain((new TimeOnly(00, 00), new TimeOnly(00, 00)));
    }
}