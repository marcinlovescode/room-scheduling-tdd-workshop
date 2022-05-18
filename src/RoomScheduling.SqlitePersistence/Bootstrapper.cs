using Microsoft.Data.Sqlite;
using Dapper;

namespace RoomScheduling.SqlitePersistence;

public class Bootstrapper
{
    private readonly Func<SqliteConnection> _createDbConnection;

    private const string CreateRoomsTable =
        @"CREATE TABLE Rooms(Name VARCHAR(255) NOT NULL PRIMARY KEY, NumberOfSeats INT NOT NULL, HasProjector INT NOT NULL, HasSoundSystem INT NOT NULL, HasAirConditioner INT NOT NULL)";
    
    private const string BookingsTable =
        @"CREATE TABLE Bookings(ResourceId VARCHAR(255) NOT NULL, [Date] VARCHAR(8) NOT NULL, [From] varchar(20) NOT NULL, [To] varchar(20) NOT NULL, PRIMARY KEY(ResourceId,[Date],
         [From],[To])
 )";

    
    public Bootstrapper(Func<SqliteConnection> createDbConnection)
    {
        _createDbConnection = createDbConnection;
    }

    public async Task Bootstrap()
    {
        await using var connection = _createDbConnection();
        await connection.ExecuteAsync(CreateRoomsTable);
        await connection.ExecuteAsync(BookingsTable);
    }
}