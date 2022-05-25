using RoomScheduling.Application.Ports;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RoomScheduling.Application.Handlers;

public class SendNotificationAboutBookingsCommand
{
    public string Name { get; }
    public DateOnly Date { get; }
    public string Email { get; }

    public SendNotificationAboutBookingsCommand(string name, DateOnly date, string email)
    {
        Name = name;
        Date = date;
        Email = email;
    }
}

public class SendNotificationAboutBookingsHandler
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IDailyScheduleDao _scheduleDao;
    private readonly string _fromEmail; 

    public SendNotificationAboutBookingsHandler(ISendGridClient sendGridClient, IDailyScheduleDao scheduleDao, string fromEmail)
    {
        _sendGridClient = sendGridClient;
        _scheduleDao = scheduleDao;
        _fromEmail = fromEmail;
    }

    public async Task Handle(SendNotificationAboutBookingsCommand command)
    {
       var schedule = await _scheduleDao.Get(command.Name, command.Date);
       var msg = new SendGridMessage()
       {
           From = new EmailAddress(_fromEmail),
           Subject = "Bookings"
       };
       var bookingsText = string.Join(", ", schedule.Bookings.Select(x => $"{x.from.ToShortTimeString()}-{x.to.ToShortTimeString()}").ToList());
       msg.HtmlContent = "Bookings: " + string.Join(", ", bookingsText);
       msg.AddTo(new EmailAddress(command.Email, "Example User"));
       await _sendGridClient.SendEmailAsync(msg, CancellationToken.None);
    }
}