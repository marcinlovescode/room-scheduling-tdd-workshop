using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RoomScheduling.Host.Dtos;
using Xunit;

namespace RoomScheduling.Host.Tests;

public class AvailableRoomsApiTests
{
    [Fact]
    public async Task Can_read_created_room()
    {
        //Arrange
        var roomApiUrl = "/api/available-rooms";
        var availableRoomsQuery = new
        {
            RequiredNumberOfSeats = 6,
            RequiresProjector = true,
            RequiresSoundSystem = false,
            RequiresAirConditioner = true,
            Date = DateTime.Now.Date
        };

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { });
        var client = application.CreateClient();
        var sqlConnection = application.Services.CreateScope().ServiceProvider.GetService<Func<SqliteConnection>>();
        if (sqlConnection == null) throw new Exception("SQL Connection is not registered");
        
        await TestDataFeeder.Feed(sqlConnection);
        var serializedQuery = JsonConvert.DeserializeObject<IDictionary<string, string?>>(JsonConvert.SerializeObject(availableRoomsQuery));
        var uri = QueryHelpers.AddQueryString(roomApiUrl, serializedQuery);
        //Act
        var response = await client.GetAsync(uri);
        //Assert
        var dto = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<AvailableRoomsDto>>();
        var availableRoom = (dto ?? throw new Exception("Response is empty")).Single();
        availableRoom.Date.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        availableRoom.Name.Should().Be(TestDataFeeder.Rooms[1].Name);
        availableRoom.HasProjector.Should().Be(TestDataFeeder.Rooms[1].HasProjector);
        availableRoom.HasAirConditioner.Should().Be(TestDataFeeder.Rooms[1].HasAirConditioner);
        availableRoom.HasSoundSystem.Should().Be(TestDataFeeder.Rooms[1].HasSoundSystem);
        availableRoom.NumberOfSeats.Should().Be(TestDataFeeder.Rooms[1].NumberOfSeats);
        availableRoom.NumberOfSeats.Should().Be(TestDataFeeder.Rooms[1].NumberOfSeats);
        availableRoom.AvailableSlots.Should().Contain((new TimeOnly(00, 00), new TimeOnly(11, 00)));
        availableRoom.AvailableSlots.Should().Contain((new TimeOnly(12, 00), new TimeOnly(16, 00)));
        availableRoom.AvailableSlots.Should().Contain((new TimeOnly(18, 00), new TimeOnly(00, 00)));
    }
}