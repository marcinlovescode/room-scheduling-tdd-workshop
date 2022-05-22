using RoomScheduling.Application.Ports;

namespace RoomScheduling.Application.Handlers;

public class FindBookingsQuery
{
    public string Name { get; }
    public DateOnly Date { get; }

    public FindBookingsQuery(string name, DateOnly date)
    {
        Name = name;
        Date = date;
    }
}

public class FindBookingsQueryResult
{
    public string Name { get; }
    public DateOnly Date { get; }
    public IReadOnlyCollection<(TimeOnly from, TimeOnly to)> Bookings { get; }

    public FindBookingsQueryResult(string name, DateOnly date, IReadOnlyCollection<(TimeOnly from, TimeOnly to)> bookings)
    {
        Name = name;
        Date = date;
        Bookings = bookings;
    }
}

public class FindBookingsHandler
{
    private readonly IDailyScheduleDao _dailyScheduleDao;

    public FindBookingsHandler(IDailyScheduleDao dailyScheduleDao)
    {
        _dailyScheduleDao = dailyScheduleDao;
    }

    public async Task<FindBookingsQueryResult> Handle(FindBookingsQuery query)
    {
        var schedule = await _dailyScheduleDao.Get(query.Name, query.Date);

        return new FindBookingsQueryResult(schedule.ResourceId, schedule.Date, schedule.Bookings.Select(x => (x.from, x.to)).ToList());
    }
}