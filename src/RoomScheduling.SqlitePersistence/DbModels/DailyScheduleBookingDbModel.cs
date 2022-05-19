using RoomScheduling.Domain;

namespace RoomScheduling.SqlitePersistence.DbModels;

public class DailyScheduleBookingDbModel
{
    public string ResourceId { get; }
    public string Date { get; }
    public string From { get; }
    public string To { get; }

    public DailyScheduleBookingDbModel(string resourceId, string date, string from, string to)
    {
        ResourceId = resourceId;
        Date = date;
        From = from;
        To = to;
    }

    public static IReadOnlyCollection<DailyScheduleBookingDbModel> FromDomain(DailySchedule dailySchedule) =>
        dailySchedule.Bookings.Select(x =>
                new DailyScheduleBookingDbModel(dailySchedule.ResourceId, dailySchedule.Date.ToShortDateString(), x.from.ToShortTimeString(), x.to.ToShortTimeString()))
            .ToList()
            .AsReadOnly();

    public static DailySchedule ToDomain(IReadOnlyCollection<DailyScheduleBookingDbModel> dailyScheduleBookingsDbModels, string resourceId, DateOnly date)
    {
        return new DailySchedule(resourceId, date,
            dailyScheduleBookingsDbModels
                .Select(x => (TimeOnly.Parse(x.From), TimeOnly.Parse(x.To)))
                .ToList
                    ());
    }
}