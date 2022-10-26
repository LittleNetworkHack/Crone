using Microsoft.Data.Sqlite;
using System.IO;

namespace Crone;
public sealed class SqliteDataProvider : CoreDataProvider<SqliteConnection, SqliteCommand, SqliteDataReader>
{
    static SqliteDataProvider()
    {
        SQLitePCL.Batteries.Init();
    }

    public override string ParameterBinder => ParametersSqlite;

    public SqliteDataProvider() : base() { }
    public SqliteDataProvider(string connectionString) : base(connectionString) { }
    public SqliteDataProvider(CoreDataOptions options) : base(options) { }

    public override string BuildConnection(string server, int port, string catalog, string username, string password)
    {
        var result = BuildConnectionSqlite(server, port, catalog, username, password);
        return result;
    }
}