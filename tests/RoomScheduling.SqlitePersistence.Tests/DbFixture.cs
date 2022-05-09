using System;
using Microsoft.Data.Sqlite;

namespace RoomScheduling.SqlitePersistence.Tests;

public static class DbFixture
{
    public static readonly string DefaultDbName = $"{Guid.NewGuid():N}";

    public static Func<SqliteConnection> GetCreateDbFunc(string dbName)
    {
        var dbConnectionString = $"Data Source={dbName}.db";
        return () => new SqliteConnection(dbConnectionString);
    }
    
    public static Func<SqliteConnection> GetDefaultCreateDbFunc() => GetCreateDbFunc(DefaultDbName);
    
}