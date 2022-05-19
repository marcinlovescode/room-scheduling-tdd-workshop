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
    
    [Theory]
    [InlineData(12,00,14,00)]
    [InlineData(13,00,13,30)]
    public void Booked_timeslot_is_visible(int fromHour, int fromMinutes, int toHour, int toMinutes)
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        var dailySchedule = new DailySchedule(id, date);
        var from = new TimeOnly(12, 00);
        var to = new TimeOnly(14, 00);
        //Act
        dailySchedule.Book(new TimeOnly(fromHour, fromMinutes), new TimeOnly(toHour, toMinutes));
        //Assert
        dailySchedule.IsTimeSlotAvailable(from, to).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(12,00,14,00)]
    [InlineData(11,00,13,00)]
    [InlineData(13,00,13,30)]
    [InlineData(13,00,16,00)]
    [InlineData(11,00,16,00)]
    public void Cannot_book_when_time_range_overlaps(int fromHour, int fromMinutes, int toHour, int toMinutes)
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        var dailySchedule = new DailySchedule(id, date);
        var from1 = new TimeOnly(12, 00);
        var from2 = new TimeOnly(fromHour, fromMinutes);
        var to1 = new TimeOnly(14, 00);
        var to2 = new TimeOnly(toHour, toMinutes);
        var errorMessage = "Time slots overlaps with existing booking";
        dailySchedule.Book(from1, to1);
        //Act
        var bookAction = () => dailySchedule.Book(from2, to2);
        //Assert
        bookAction.Should().Throw<InvalidOperationException>().WithMessage(errorMessage);
    }
    
    [Fact]
    public void Cannot_book_when_time_range_overlaps_composition_of_two_time_ranges()
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 9);
        var dailySchedule = new DailySchedule(id, date);
        var from1 = new TimeOnly(10, 00);
        var to1 = new TimeOnly(12, 00);
        var from2 = new TimeOnly(13, 00);
        var to2 = new TimeOnly(14, 00);
        var from3 = new TimeOnly(12, 30);
        var to3 = new TimeOnly(13, 30);
        var errorMessage = "Time slots overlaps with existing booking";
        dailySchedule.Book(from1, to1);
        dailySchedule.Book(from2, to2);
        //Act
        var bookAction = () => dailySchedule.Book(from3, to3);
        //Assert
        bookAction.Should().Throw<InvalidOperationException>().WithMessage(errorMessage);
    }
    
    [Fact]
    public void Returns_available_time_slots()
    {
        //Arrange
        var id = Guid.NewGuid().ToString("N");
        var date = new DateOnly(2022, 5, 18);
        var dailySchedule = new DailySchedule(id, date);
        var from1 = new TimeOnly(10, 00);
        var to1 = new TimeOnly(12, 00);
        var from2 = new TimeOnly(13, 00);
        var to2 = new TimeOnly(14, 00);
        dailySchedule.Book(from1, to1);
        dailySchedule.Book(from2, to2);
        //Act
        var result = dailySchedule.AvailableTimeSlots();
        //Assert
        result[0].Should.Be((new TimeOnly(00, 00), new TimeOnly(10, 00)));
        result[1].Should.Be((new TimeOnly(12, 00), new TimeOnly(13, 00)));
        result[2].Should.Be((new TimeOnly(14, 00), new TimeOnly(00, 00)));
    }
}