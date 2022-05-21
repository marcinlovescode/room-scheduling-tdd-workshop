using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RoomScheduling.Domain;
using RoomScheduling.SqlitePersistence;

namespace RoomScheduling.Host.Tests;

public static class TestDataFeeder
{
    public static readonly Room[] Rooms =
    {
        new(5, true, true, true, $"{Guid.NewGuid():N}"),
        new(6, true, false, true, $"{Guid.NewGuid():N}"),
        new(7, false, false, false, $"{Guid.NewGuid():N}"),
        new(8, false, true, true, $"{Guid.NewGuid():N}")
    };
    public static readonly DailySchedule[] DailySchedules =
    {
        new(Rooms[0].Name, DateOnly.FromDateTime(DateTime.Now),
            new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(10, 00), new TimeOnly(13, 00)), (new TimeOnly(14, 30), new TimeOnly(16, 00)) }),
        new(Rooms[1].Name, DateOnly.FromDateTime(DateTime.Now),
            new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(11, 00), new TimeOnly(12, 00)), (new TimeOnly(16, 00), new TimeOnly(18, 00)) }),
        new(Rooms[2].Name, DateOnly.FromDateTime(DateTime.Now), new (TimeOnly from, TimeOnly to)[] { (new TimeOnly(08, 00), new TimeOnly(15, 00)) })
    };
    
    public static async Task Feed(Func<SqliteConnection> sqlConnection)
    {

        var roomDao = new RoomDao(sqlConnection);
        var scheduleDao = new DailyScheduleDao(sqlConnection);
        await Task.WhenAll(Rooms.Select(async x => await roomDao.Save(x)));
        await Task.WhenAll(DailySchedules.Select(async x => await scheduleDao.Save(x)));
    }
}