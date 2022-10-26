using Microsoft.Data.SqlClient;

namespace Crone;
public sealed class SqlServerDataProvider : CoreDataProvider<SqlConnection, SqlCommand, SqlDataReader>
{
    public override string ParameterBinder => ParametersMSSQL;

    public SqlServerDataProvider() : base() { }
    public SqlServerDataProvider(string connectionString) : base(connectionString) { }
    public SqlServerDataProvider(CoreDataOptions options) : base(options) { }

    public override string BuildConnection(string server, int port, string catalog, string username, string password)
    {
        var result = BuildConnectionSqlServer(server, port, catalog, username, password);
        return result;
    }

    public override SqlCommand DeriveCommandParameters(SqlCommand command)
    {
        SqlCommandBuilder.DeriveParameters(command);
        return command;
    }
}
