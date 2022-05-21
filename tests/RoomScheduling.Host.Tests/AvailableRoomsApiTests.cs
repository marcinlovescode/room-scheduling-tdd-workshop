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
    public async Task Returns_available_rooms()
    {
        //Arrange
        var roomApiUrl = "/api/availablerooms";
        var availableRoomsQuery = new
        {
            RequiredNumberOfSeats = 6,
            RequiresProjector = true,
            RequiresSoundSystem = false,
            RequiresAirConditioner = true,
            Date = DateTime.Now
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
        availableRoom.Date.Date.Should().Be(DateTime.Now.Date);
        availableRoom.Name.Should().Be(TestDataFeeder.Rooms[1].Name);
        availableRoom.HasProjector.Should().Be(TestDataFeeder.Rooms[1].HasProjector);
        availableRoom.HasAirConditioner.Should().Be(TestDataFeeder.Rooms[1].HasAirConditioner);
        availableRoom.HasSoundSystem.Should().Be(TestDataFeeder.Rooms[1].HasSoundSystem);
        availableRoom.NumberOfSeats.Should().Be(TestDataFeeder.Rooms[1].NumberOfSeats);
        availableRoom.AvailableSlots[0].From.Should().Be("00:00");
        availableRoom.AvailableSlots[0].To.Should().Be("11:00");
        availableRoom.AvailableSlots[1].From.Should().Be("12:00");
        availableRoom.AvailableSlots[1].To.Should().Be("16:00");        
        availableRoom.AvailableSlots[2].From.Should().Be("18:00");
        availableRoom.AvailableSlots[2].To.Should().Be("00:00");
    }
}