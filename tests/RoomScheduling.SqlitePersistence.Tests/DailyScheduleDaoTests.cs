using System;
using System.Threading.Tasks;
using FluentAssertions;
using RoomScheduling.Domain;
using RoomScheduling.Fixtures;
using Xunit;

namespace RoomScheduling.SqlitePersistence.Tests;

public class DailyScheduleDaoTests
{
    [Fact]
    public async Task Saves_resource_booking()
    {
        //Arrange
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        var dbBootstrapper = new Bootstrapper(createDbConnection);
        await dbBootstrapper.Bootstrap();
        var dailySchedule = new DailySchedule($"{Guid.NewGuid():N}", DateOnly.FromDateTime(DateTime.Now));
        var from = new TimeOnly(10, 00);
        var to = new TimeOnly(12, 00);
        dailySchedule.Book(new TimeOnly(10, 00), new TimeOnly(12, 00));
        var dao = new DailyScheduleDao(createDbConnection);
        //Act
        await dao.Save(dailySchedule);
        //Assert
        var dailyScheduleFromDb = await dao.Get(dailySchedule.ResourceId, dailySchedule.Date);
        dailyScheduleFromDb.IsTimeSlotAvailable(from, to).Should().BeFalse();
    }
    
    [Fact]
    public async Task Returns_fully_available_schedule_when_no_bookings_made()
    {
        //Arrange
        var createDbConnection = DbFixture.GetDefaultCreateDbFunc();
        var dbBootstrapper = new Bootstrapper(createDbConnection);
        var notExistingResourceId = $"{Guid.NewGuid():N}";
        var anyDate = DateOnly.MinValue;
        await dbBootstrapper.Bootstrap();
        var dao = new DailyScheduleDao(createDbConnection);
        //Act
        var result = await dao.Get(notExistingResourceId, DateOnly.MinValue);
        //Assert
        result.Bookings.Should().HaveCount(0);
        result.ResourceId.Should().Be(notExistingResourceId);
        result.Date.Should().Be(anyDate);
    }
}