using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Xunit;

namespace RoomScheduling.Host.Tests;

public class NotificationsApiTests
{
     [Fact]
    public async Task Output_email_contains_bookings()
    {
        //Arrange
        var roomName = TestDataFeeder.Rooms[0].Name;
        var notificationsUrl = $"/api/notifications/bookings/rooms/{roomName}";
        var sendEmailNotificationCommand = new
        {
            Date = DateTime.Now,
            Email = "my-email@test.com",
        };
        
        var sendGridMock = new Mock<ISendGridClient>();
        var sendGridResponse = new Response(HttpStatusCode.Accepted, new StringContent("OK"), null);
        SendGridMessage? sendGridMessageToAssert = null;
        sendGridMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .Callback((SendGridMessage message, CancellationToken _) => { sendGridMessageToAssert = message; })
            .Returns(Task.FromResult(sendGridResponse));

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(service =>
                {
                    service.AddScoped(_ => sendGridMock.Object);
                });
            });
        var client = application.CreateClient();
        var sqlConnection = application.Services.CreateScope().ServiceProvider.GetService<Func<SqliteConnection>>();
        if (sqlConnection == null) throw new Exception("SQL Connection is not registered");
        
        await TestDataFeeder.Feed(sqlConnection);
        var request = new HttpRequestMessage(HttpMethod.Post, notificationsUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(sendEmailNotificationCommand), Encoding.Default, "application/json");
        //Act
        var response = await client.SendAsync(request);
        //Assert
        response.EnsureSuccessStatusCode();
        if (sendGridMessageToAssert == null)
            throw new Exception("SendGrid client not called");
        var emails = sendGridMessageToAssert.Personalizations.SelectMany(x => x.Tos).Select(x => x.Email).ToList();
        emails[0].Should().Be(sendEmailNotificationCommand.Email);
        sendGridMessageToAssert.HtmlContent.Should().Be("Bookings: 10:00-13:00, 14:30-16:00");
    }
}