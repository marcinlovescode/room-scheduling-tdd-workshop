using System;
using Microsoft.Data.Sqlite;

namespace RoomScheduling.Fixtures;

public static class DbFixture
{
    private static string GetDefaultDbName() => $"{Guid.NewGuid():N}";

    public static Func<SqliteConnection> GetCreateDbFunc(string dbName)
    {
        var dbConnectionString = $"Data Source={dbName}.db";
        return () => new SqliteConnection(dbConnectionString);
    }
    
    public static Func<SqliteConnection> GetDefaultCreateDbFunc() => GetCreateDbFunc(GetDefaultDbName());
    
}