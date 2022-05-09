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
    
    [Fact]
    public void Booked_timeslot_is_visible()
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        var dailySchedule = new DailySchedule(id, date);
        var from = new TimeOnly(12, 00);
        var to = new TimeOnly(14, 00);
        //Act
        dailySchedule.Book(from, to);
        //Assert
        dailySchedule.IsTimeSlotAvailable(from, to).Should().BeFalse();
    }
    
    [Fact]
    public void Cannot_book_when_time_range_overlaps()
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        var dailySchedule = new DailySchedule(id, date);
        var from1 = new TimeOnly(12, 00);
        var from2 = new TimeOnly(12, 00);
        var to1 = new TimeOnly(14, 00);
        var to2 = new TimeOnly(14, 00);
        var errorMessage = "Time slots overlaps with existing booking";
        dailySchedule.Book(from1, to1);
        //Act
        var bookAction = () => dailySchedule.Book(from2, to2);
        //Assert
        bookAction.Should().Throw<InvalidOperationException>().WithMessage(errorMessage);
    }
}