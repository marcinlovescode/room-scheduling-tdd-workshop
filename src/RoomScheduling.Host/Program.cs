using Microsoft.Data.Sqlite;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;
using RoomScheduling.SqlitePersistence;

var builder = WebApplication.CreateBuilder(args);
var dbName = Guid.NewGuid().ToString("N");
var createDbFunc = () => new SqliteConnection($"Data Source={dbName}.db");

builder.Services.AddControllers();
builder.Services.AddScoped<Func<SqliteConnection>>(_ => createDbFunc);
builder.Services.AddScoped<Func<DefineRoomCommand, Task>>(provider =>
{
    var roomDao = provider.GetService<IRoomDao>();
    if (roomDao == null)
        throw new ArgumentNullException(nameof(roomDao));
    return new DefineRoomHandler(roomDao).Handle;
});
builder.Services.AddScoped<Func<string, Task<Room>>>(provider =>
{
    var roomDao = provider.GetService<IRoomDao>();
    if (roomDao == null)
        throw new ArgumentNullException(nameof(roomDao));
    return new ReadRoomHandler(roomDao).Handle;
});
builder.Services.AddScoped<Func<FindAvailableSlotsForRoomsMatchingCriteriaQuery, Task<IReadOnlyCollection<FindAvailableSlotsForRoomsMatchingCriteriaQueryResult>>>>(provider =>
{
    var roomDao = provider.GetService<IRoomDao>();
    var dailyScheduleDao = provider.GetService<IDailyScheduleDao>();

    if (roomDao == null)
        throw new ArgumentNullException(nameof(roomDao));
    if (dailyScheduleDao == null)
        throw new ArgumentNullException(nameof(dailyScheduleDao));
    return new FindAvailableSlotsForRoomsMatchingCriteriaHandler(roomDao, dailyScheduleDao).Handle;
});
builder.Services.AddScoped<Func<FindBookingsQuery, Task<FindBookingsQueryResult>>>(provider =>
{
    var dailyScheduleDao = provider.GetService<IDailyScheduleDao>();

    if (dailyScheduleDao == null)
        throw new ArgumentNullException(nameof(dailyScheduleDao));
    return new FindBookingsHandler(dailyScheduleDao).Handle;
});
builder.Services.AddScoped<Func<BookRoomCommand, Task>>(provider =>
{
    var dailyScheduleDao = provider.GetService<IDailyScheduleDao>();

    if (dailyScheduleDao == null)
        throw new ArgumentNullException(nameof(dailyScheduleDao));
    return new BookRoomHandler(dailyScheduleDao).Handle;
});
builder.Services.AddScoped<IRoomDao, RoomDao>();
builder.Services.AddScoped<IDailyScheduleDao, DailyScheduleDao>();

var app = builder.Build();

app.MapControllers();
new Bootstrapper(createDbFunc).Bootstrap().GetAwaiter().GetResult();
app.Run();

public partial class Program
{
}