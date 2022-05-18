using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
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
    }
}