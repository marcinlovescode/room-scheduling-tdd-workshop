using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RoomScheduling.Host.Dtos;
using Xunit;

namespace RoomScheduling.Host.Tests
{
    public class RoomApiTests
    {
        [Fact]
        public async Task Can_read_created_room()
        {
            //Arrange
            var roomApiUrl = "/api/rooms";
            var roomName = $"{Guid.NewGuid():N}";
            var payload = new RoomDto
            {
                Name = roomName,
                NumberOfSeats = 5,
                HasSoundSystem = true,
                HasAirConditioner = true,
                HasProjector = true
            };
            var request = new HttpRequestMessage(HttpMethod.Post, roomApiUrl);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.Default, "application/json");

            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                }); 
            var client = application.CreateClient();
            var response = await client.SendAsync(request);
            var location = response.Headers.Location;
            //Act
            response.EnsureSuccessStatusCode();
            var result = await client.GetAsync(location);
            //Assert
            var dto = await result.Content.ReadFromJsonAsync<RoomDto>();
            dto.Should().NotBeNull();
            dto?.Name.Should().Be(payload.Name);
            dto?.NumberOfSeats.Should().Be(payload.NumberOfSeats);
            dto?.HasProjector.Should().Be(payload.HasProjector);
            dto?.HasSoundSystem.Should().Be(payload.HasSoundSystem);
            dto?.HasAirConditioner.Should().Be(payload.HasAirConditioner);
        }
        
        [Fact]
        public async Task Can_book_room()
        {
            //Arrange
            var roomName = TestDataFeeder.Rooms[0].Name;
            var roomApiUrl = $"/api/rooms/{roomName}/bookings";
            var payload = new
            {
                Date = DateTime.Now,
                From = "08:00",
                To = "09:00"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, roomApiUrl);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.Default, "application/json");

            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                }); 
            var sqlConnection = application.Services.CreateScope().ServiceProvider.GetService<Func<SqliteConnection>>();
            if (sqlConnection == null) throw new Exception("SQL Connection is not registered");
        
            await TestDataFeeder.Feed(sqlConnection);

            var client = application.CreateClient();
            var response = await client.SendAsync(request);
            var location = response.Headers.Location;
            //Act
            response.EnsureSuccessStatusCode();
            var result = await client.GetAsync(location);
            //Assert
            var dto = await result.Content.ReadFromJsonAsync<BookingsDto>();
            dto.Should().NotBeNull();
            dto?.Name.Should().Be(roomName);
            dto?.Date.Date.Should().Be(DateTime.Now.Date);
            dto?.Bookings.Single(x => x.From == "08:00" && x.To == "09:00").Should().NotBeNull();
        }
    }
}