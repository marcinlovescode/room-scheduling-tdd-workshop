using RoomScheduling.Application.Ports;

namespace RoomScheduling.Application.Handlers;

public class BookRoomCommand
{
    public DateOnly Date { get; }
    public string Name { get; }
    public TimeOnly From { get; }
    public TimeOnly To { get; }

    public BookRoomCommand(DateOnly date, string name, TimeOnly from, TimeOnly to)
    {
        Date = date;
        Name = name;
        From = from;
        To = to;
    }
}

public class BookRoomHandler
{
    private readonly IDailyScheduleDao _dailyScheduleDao;

    public BookRoomHandler(IDailyScheduleDao dailyScheduleDao)
    {
        _dailyScheduleDao = dailyScheduleDao;
    }

    public async Task Handle(BookRoomCommand command)
    {
        var dailySchedule = await _dailyScheduleDao.Get(command.Name, command.Date);
        dailySchedule.Book(command.From, command.To);
        await _dailyScheduleDao.Save(dailySchedule);
    }
}