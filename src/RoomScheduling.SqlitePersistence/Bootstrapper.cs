using Microsoft.Data.Sqlite;
using Dapper;

namespace RoomScheduling.SqlitePersistence;

public class Bootstrapper
{
    private readonly Func<SqliteConnection> _createDbConnection;

    private const string CreateRoomsTable =
        @"CREATE TABLE Rooms(Name VARCHAR(255) NOT NULL, NumberOfSeats INT NOT NULL, HasProjector INT NOT NULL, HasSoundSystem INT NOT NULL, HasAirConditioner INT NOT NULL)";
    
    public Bootstrapper(Func<SqliteConnection> createDbConnection)
    {
        _createDbConnection = createDbConnection;
    }

    public async Task Bootstrap()
    {
        await using var connection = _createDbConnection();
        await connection.ExecuteAsync(CreateRoomsTable);
    }
}