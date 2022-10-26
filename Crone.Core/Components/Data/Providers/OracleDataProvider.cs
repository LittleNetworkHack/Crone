using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Crone;
public sealed class OracleDataProvider : CoreDataProvider<OracleConnection, OracleCommand, OracleDataReader>
{
    public static OracleRefCursor Cursor => OracleRefCursor.Null;

    public override string ParameterBinder => ParameterOracle;

    public OracleDataProvider() : base() { }
    public OracleDataProvider(string connectionString) : base(connectionString) { }
    public OracleDataProvider(CoreDataOptions options) : base(options) { }

    public override string BuildConnection(string server, int port, string catalog, string username, string password)
    {
        var result = BuildConnectionOracle(server, port, catalog, username, password);
        return result;
    }

    public override OracleCommand DeriveCommandParameters(OracleCommand command)
    {
        OracleCommandBuilder.DeriveParameters(command);
        return command;
    }
    public override OracleCommand BindCommandByName(OracleCommand command, CoreDataParameters parameters, ParameterDirection? direction = null)
    {
        command.BindByName = true;
        base.BindCommandByName(command, parameters, direction);
        return command;
    }

    public override OracleDataReader ExecuteReader(OracleCommand command)
    {
        var result = base.ExecuteReader(command);
        result.SuppressGetDecimalInvalidCastException = true;
        return result;
    }

    public override OracleDataReader ExecuteReader(OracleCommand command, int index)
    {
        command.ExecuteNonQuery();
        var error = new ArgumentException("Specified cursor not found in parameters collection!");
        var item = command.Parameters[index] ?? throw error;
        var result = GetCursorReader(item) ?? throw error;
        result.SuppressGetDecimalInvalidCastException = true;
        return result;
    }

    public override OracleDataReader ExecuteReader(OracleCommand command, string name)
    {
        command.ExecuteNonQuery();
        var error = new ArgumentException("Specified cursor not found in parameters collection!");
        var item = command.Parameters[name] ?? throw error;
        var result = GetCursorReader(item) ?? throw error;
        result.SuppressGetDecimalInvalidCastException = true;
        return result;
    }

    public OracleDataReader GetCursorReader(OracleParameter parameter)
    {
        var result = (parameter?.Value) switch
        {
            OracleDataReader reader => reader,
            OracleRefCursor cursor => cursor.GetDataReader(),
            _ => null,
        };
        return result;
    }
    public override CoreDataSet ExecuteDataSet(OracleConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        command.ExecuteNonQuery();
        var result = new CoreDataSet();
        foreach (OracleParameter arg in command.Parameters)
        {
            if (!arg.Direction.HasFlag(ParameterDirection.Output))
            {
                continue;
            }
            if (arg.OracleDbType == OracleDbType.RefCursor)
            {
                using var reader = GetCursorReader(arg);
                var table = GetDataTable(reader);
                result.Add(arg.ParameterName, table);
                continue;
            }
            result.Add(arg.ParameterName, arg.Value);
        }
        return result;
    }
}
