using System;
using FluentAssertions;
using Xunit;

namespace RoomScheduling.Domain.Tests;

public class DailyScheduleTests
{
    [Fact]
    public void DailySchedule_has_resource_identifier_and_date()
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        //Act
        var dailySchedule = new DailySchedule(id, date);
        //Assert
        dailySchedule.ResourceId.Should().Be(id);
        dailySchedule.Date.Should().Be(date);
    }
}