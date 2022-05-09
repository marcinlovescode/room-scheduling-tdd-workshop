using Dapper;
using Microsoft.Data.Sqlite;
using RoomScheduling.Application.Ports;
using RoomScheduling.Domain;
using RoomScheduling.SqlitePersistence.DbModels;

namespace RoomScheduling.SqlitePersistence;

public class RoomDao : IRoomDao
{
    private readonly Func<SqliteConnection> _createDbConnection;

    private const string InsertSqlCommand =
        @"INSERT INTO Rooms (Name, NumberOfSeats, HasProjector, HasSoundSystem, HasAirConditioner)
          VALUES(@Name, @NumberOfSeats, @HasProjector, @HasSoundSystem, @HasAirConditioner)";

    private const string ReadSqlQuery =
        @"SELECT NumberOfSeats, HasProjector, HasSoundSystem, HasAirConditioner, Name
          FROM Rooms
          WHERE Name=@name";

    public RoomDao(Func<SqliteConnection> createDbConnection)
    {
        _createDbConnection = createDbConnection;
    }

    public async Task Save(Room room)
    {
        await using var connection = _createDbConnection();
        await connection.ExecuteAsync(InsertSqlCommand, RoomDbModel.FromDomain(room));
    }
    
    public async Task<Room> Get(string name)
    {
        await using var connection = _createDbConnection();
        var roomDbModel = await connection.QuerySingleAsync<RoomDbModel>(ReadSqlQuery, new
        {
            name
        });
        return roomDbModel.ToDomain();
    }
}