using Microsoft.Data.Sqlite;
using RoomScheduling.Application.Handlers;
using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;
using RoomScheduling.SqlitePersistence;
using SendGrid;

var sendGridApiKey = "<SECRET>";
var fromEmail = "marcin@marcinlovescode.com";

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
builder.Services.AddScoped<Func<SendNotificationAboutBookingsCommand, Task>>(provider =>
{
    var dailyScheduleDao = provider.GetService<IDailyScheduleDao>();
    var sendGridClient = provider.GetService<ISendGridClient>();

    if (dailyScheduleDao == null)
        throw new ArgumentNullException(nameof(dailyScheduleDao));
    if (sendGridClient == null)
        throw new ArgumentNullException(nameof(dailyScheduleDao));
    return new SendNotificationAboutBookingsHandler(sendGridClient, dailyScheduleDao, fromEmail).Handle;
});
builder.Services.AddScoped<Func<Task<IReadOnlyCollection<Room>>>>(provider =>
{
    var roomDao = provider.GetService<IRoomDao>();
    if (roomDao == null)
        throw new ArgumentNullException(nameof(roomDao));
    return new ReadRoomsHandler(roomDao).Handle;
});
builder.Services.AddScoped<IRoomDao, RoomDao>();
builder.Services.AddScoped<IDailyScheduleDao, DailyScheduleDao>();
builder.Services.AddScoped<ISendGridClient>(_ => new SendGridClient(sendGridApiKey));
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("cors", builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Location");
    }));

var app = builder.Build();

app.MapControllers();
new Bootstrapper(createDbFunc).Bootstrap().GetAwaiter().GetResult();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsEnvironment("E2E-TESTS"))
{
   var roomDao =  app.Services.GetService<IRoomDao>();
   if (roomDao == null)
        throw new ArgumentNullException(nameof(roomDao));
   roomDao.Save(new Room(10, true, true, true, "Burning Desire")).GetAwaiter().GetResult();
   roomDao.Save(new Room(5, true, false, true, "Fortune Seekers")).GetAwaiter().GetResult();
   roomDao.Save(new Room(15, false, true, true, "Goal")).GetAwaiter().GetResult();
   roomDao.Save(new Room(8, true, true, false, "Think Out Loud")).GetAwaiter().GetResult();
}

app.UseCors("cors");

app.Run();

public partial class Program
{
}