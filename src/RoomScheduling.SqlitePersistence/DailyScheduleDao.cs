using Dapper;
using Microsoft.Data.Sqlite;
using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;
using RoomScheduling.SqlitePersistence.DbModels;

namespace RoomScheduling.SqlitePersistence;

public class DailyScheduleDao : IDailyScheduleDao
{
    private readonly Func<SqliteConnection> _createDbConnection;

    private const string InsertSqlCommand =
        @"INSERT INTO Bookings (ResourceId, [Date], [From], [To])
          VALUES(@ResourceId, @Date, @From, @To)";

    private const string ReadSqlQuery =
        @"SELECT ResourceId, [Date], [From], [To]
          FROM Bookings
          WHERE ResourceId in @ResourcesIds AND [Date]=@Date";

    public DailyScheduleDao(Func<SqliteConnection> createDbConnection)
    {
        _createDbConnection = createDbConnection;
    }

    public async Task Save(DailySchedule dailySchedule)
    {
        await using var connection = _createDbConnection();
        await connection.ExecuteAsync(InsertSqlCommand, DailyScheduleBookingDbModel.FromDomain(dailySchedule));
    }

    public async Task<DailySchedule> Get(string resourceId, DateOnly date)
    {
        await using var connection = _createDbConnection();
        var roomDbModel = await connection.QueryAsync<DailyScheduleBookingDbModel>(ReadSqlQuery, new
        {
            ResourceId = resourceId,
            Date = date.ToShortDateString()
        });
        return DailyScheduleBookingDbModel.ToDomain(roomDbModel.ToList(), resourceId, date);
    }

    public async Task<IReadOnlyCollection<DailySchedule>> Get(string[] resources, DateOnly date)
    {
        await using var connection = _createDbConnection();
        var roomDbModel = (await connection.QueryAsync<DailyScheduleBookingDbModel>(ReadSqlQuery, new
        {
            ResourcesIds = resources,
            Date = date.ToShortDateString()
        })).ToList();
        var notExistingBookings = resources.Except(roomDbModel.Select(x => x.ResourceId).Distinct())
            .Select(x => DailyScheduleBookingDbModel.ToDomain(new DailyScheduleBookingDbModel[] { }, x, date));
        var bookingsByRoom = roomDbModel.GroupBy(x => x.ResourceId).ToDictionary(x => x.Key, x => x.ToList())
            .Select(x => DailyScheduleBookingDbModel.ToDomain(x.Value, x.Key, date));
        return notExistingBookings.Concat(bookingsByRoom).ToList();
    }
}