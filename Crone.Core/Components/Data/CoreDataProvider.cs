using static Crone.CoreLib;
namespace Crone;

public abstract class CoreDataProvider : CoreComponent, ICoreDataProvider
{
    #region Static

    public const string ParameterPlaceholder = "~@";
    public const string ParameterOracle = ":";
    public const string ParametersMSSQL = "@";
    public const string ParametersSqlite = "@";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FixOutputName(string name)
    {
        return name?.Split(' ').Last();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FixExpressionName(string name)
    {
        return name?.Replace(".", "_");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object FormatCommandExpression<T>(T value, string format)
    {
        if (value is IFormattable formattable && string.IsNullOrEmpty(format) == false)
        {
            return formattable.ToString(format, CultureInfo.InvariantCulture);
        }
        return value;
    }

    public static string BuildConnectionOracle(string server, int port, string catalog, string username, string password)
    {
        var sb = new StringBuilder();
        sb.Append($"DATA SOURCE=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port}))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={catalog})));");
        sb.Append($"USER ID={username};PASSWORD={password};");
        sb.Append($"PERSIST SECURITY INFO=true;POOLING=true;");
        // To avoid some 'new default' exceptions that slow down opening sql connection when they throw, which is always...
        // What are they, do we need them on 'true'?  No ameri-care.... they just throw... thanks obama
        sb.Append($"LOAD BALANCING=false;HA EVENTS=false;");
        var result = sb.ToString();
        return result;
    }
    public static string BuildConnectionSqlServer(string server, int port, string catalog, string username, string password)
    {
        var sb = new StringBuilder();
        sb.Append($"Server={server},{port};Database={catalog};User Id={username};Password={password};");
        // MARS is used if you want to execute 2 (or more) commands in parallel
        // Async is here if there is a chance somebody needs it
        sb.Append($"MultipleActiveResultSets=true;Asynchronous Processing=True;");
        var result = sb.ToString();
        return result;
    }
    public static string BuildConnectionSqlite(string server, int port, string catalog, string username, string password)
    {
        var result = string.IsNullOrEmpty(password) ? $"Data Source={catalog};" : $"Data Source={catalog};Password={password};";
        return result;
    }

    #endregion Static

    #region Parameters

    public abstract string ParameterBinder { get; }
    public virtual CoreDataOptions Options { get; }

    #endregion Parameters

    #region Constructors

    public CoreDataProvider()
    {
        Options = new CoreDataOptions();
    }

    public CoreDataProvider(string connectionString)
    {
        Options = new CoreDataOptions
        {
            ConnectionString = connectionString
        };
    }

    public CoreDataProvider(CoreDataOptions options)
    {
        Options = options ?? new CoreDataOptions();
    }

    #endregion Constructors

    #region Methods

    public abstract string BuildConnection(string server, int port, string catalog, string username, string password);

    public abstract int ExecuteNonQuery(ICoreDataCommand coreCommand);
    public abstract T ExecuteValueFirst<T>(ICoreDataCommand coreCommand);
    public abstract List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand);
    public abstract List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, int index);
    public abstract List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, string name);
    public abstract List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand);
    public abstract List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, int index);
    public abstract List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, string name);
    public abstract CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand);
    public abstract CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, int index);
    public abstract CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, string name);
    public abstract CoreDataSet ExecuteDataSet(ICoreDataCommand coreCommand);

    #endregion Methods
}

public abstract class CoreDataProvider<TConnection, TCommand, TReader> : CoreDataProvider, ICoreDataProvider<TConnection, TCommand, TReader>
    where TConnection : DbConnection
    where TCommand : DbCommand
    where TReader : DbDataReader
{
    #region Constructors

    public CoreDataProvider() : base() { }
    public CoreDataProvider(string connectionString) : base(connectionString) { }
    public CoreDataProvider(CoreDataOptions options) : base(options) { }

    #endregion Constructors

    #region Connection

    public virtual TConnection CreateConnection()
    {
        var result = CreateConnection(Options);
        return result;
    }
    public virtual TConnection CreateConnection(CoreDataOptions options)
    {
        var result = CreateConnection(options.ConnectionString);
        return result;
    }
    public virtual TConnection CreateConnection(string server, int port, string catalog, string username, string password)
    {
        var connectionString = BuildConnection(server, port, catalog, username, password);
        var result = CreateConnection(connectionString);
        return result;
    }
    public virtual TConnection CreateConnection(string connectionString)
    {
        TConnection connection = null;
        try
        {
            connection = Create<TConnection>();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
        catch
        {
            connection.TryDispose();
            throw;
        }
    }

    #endregion Connection

    #region Command

    public virtual TCommand CreateCommand(TConnection connection)
    {
        var result = (TCommand)connection.CreateCommand();
        return result;
    }
    public virtual TCommand CreateCommand(TConnection connection, ICoreDataCommand command)
    {
        TCommand result = null;
        try
        {
            var parameters = command.Parameters;
            result = CreateCommand(connection);
            result = SetupCommandText(result, command);
            result = SetupCommandType(result, command);
            result = SetupCommandParameters(result, command);
            return result;
        }
        catch
        {
            result.TryDispose();
            throw;
        }
    }

    protected virtual TCommand SetupCommandText(TCommand result, ICoreDataCommand command)
    {
        result.CommandText = command.IsProcedure ? command.Text : command.Text.Replace(ParameterPlaceholder, ParameterBinder);
        return result;
    }
    protected virtual TCommand SetupCommandType(TCommand result, ICoreDataCommand command)
    {
        result.CommandType = command.IsProcedure ? CommandType.StoredProcedure : CommandType.Text;
        return result;
    }
    protected virtual TCommand SetupCommandParameters(TCommand result, ICoreDataCommand command)
    {
        var direction = command.IsProcedure ? ParameterDirection.InputOutput : default(ParameterDirection?);
        result = command.DeriveParameters ? DeriveCommandParameters(result) : result;
        result = command.BindByName ? BindCommandByName(result, command.Parameters, direction) : BindCommandByPosition(result, command.Parameters, direction);
        return result;
    }

    public virtual TCommand DeriveCommandParameters(TCommand command)
    {
        return command;
    }

    public virtual TCommand BindCommandByName(TCommand command, CoreDataParameters parameters, ParameterDirection? direction = default)
    {
        foreach (var (name, value) in parameters)
        {
            SetParameter(command, name, value, direction);
        }
        return command;
    }
    public virtual TCommand BindCommandByPosition(TCommand command, CoreDataParameters parameters, ParameterDirection? direction = default)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            SetParameter(command, i, parameters[i], direction);
        }
        return command;
    }

    public virtual T GetParameter<T>(TCommand command, string name)
    {
        var value = command.Parameters.Contains(name) ? command.Parameters[name].Value : null;
        var result = ConvertTo<T>(value);
        return result;
    }
    public virtual void SetParameter(TCommand command, string name, object value, ParameterDirection? direction = null)
    {
        if (command.Parameters.Contains(name))
        {
            var existing = command.Parameters[name];
            existing.Value = value ?? DBNull.Value;
            existing.Direction = direction ?? existing.Direction;
            return;
        }
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        parameter.Direction = direction ?? parameter.Direction;
        command.Parameters.Add(parameter);
    }

    public virtual T GetParameter<T>(TCommand command, int index)
    {
        var value = command.Parameters.Count.IndexInRange(index) ? command.Parameters[index].Value : null;
        var result = ConvertTo<T>(value);
        return result;
    }
    public virtual void SetParameter(TCommand command, int index, object value, ParameterDirection? direction = null)
    {
        if (command.Parameters.Count.IndexInRange(index))
        {
            var existing = command.Parameters[index];
            existing.Value = value ?? DBNull.Value;
            existing.Direction = direction ?? existing.Direction;
            return;
        }
        var parameter = command.CreateParameter();
        parameter.Value = value ?? DBNull.Value;
        parameter.Direction = direction ?? parameter.Direction;
        command.Parameters.Insert(index, parameter);
    }

    #endregion Command

    #region Reader

    public virtual T GetValue<T>(TReader reader, int index)
    {
        var value = reader[index];
        var result = ConvertTo<T>(value);
        return result;
    }
    public virtual object GetValue(TReader reader, int index)
    {
        var value = reader[index];
        var result = value == DBNull.Value ? null : value;
        return result;
    }
    public virtual object GetValue(TReader reader, int index, Type type)
    {
        var value = reader[index];
        var result = ConvertTo(value, type);
        return result;
    }

    public virtual T GetValue<T>(TReader reader, string name)
    {
        var value = reader[name];
        var result = ConvertTo<T>(value);
        return result;
    }
    public virtual object GetValue(TReader reader, string name)
    {
        var value = reader[name];
        var result = value == DBNull.Value ? null : value;
        return result;
    }
    public virtual object GetValue(TReader reader, string name, Type type)
    {
        var value = reader[name];
        var result = ConvertTo(value, type);
        return result;
    }

    public virtual T GetValueFirst<T>(TReader reader)
    {
        if (!reader.HasRows)
        {
            return default;
        }
        if (!reader.Read())
        {
            return default;
        }
        var result = GetValue<T>(reader, 0);
        return result;
    }
    public virtual List<T> GetValueList<T>(TReader reader)
    {
        if (!reader.HasRows)
        {
            return new List<T>();
        }
        var result = new List<T>();
        while (reader.Read())
        {
            var item = GetValue<T>(reader, 0);
            result.Add(item);
        }
        return result;
    }

    public virtual string[] GetFieldNames(TReader reader)
    {
        var count = reader.FieldCount;
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = reader.GetName(i);
        }
        return result;
    }
    public virtual PropertyInfo[] GetFieldProperties<T>(TReader reader)
    {
        var count = reader.FieldCount;
        var result = new PropertyInfo[count];
        var properties = GetPropertiesMap<T>();
        for (var i = 0; i < count; i++)
        {
            var name = reader.GetName(i);
            var prop = properties.TryGetValue(name, out var value) ? value : null;
            result[i] = prop;
        }
        return result;
    }
    public virtual CoreDataReaderColumn[] GetFieldNamesAndProperties<T>(TReader reader)
    {
        var count = reader.FieldCount;
        var result = new CoreDataReaderColumn[count];
        var properties = GetPropertiesMap<T>();
        for (var i = 0; i < count; i++)
        {
            var name = reader.GetName(i);
            var prop = properties.TryGetValue(name, out var value) ? value : null;
            result[i] = new CoreDataReaderColumn(name, prop);
        }
        return result;
    }

    public virtual List<T> GetRecordList<T>(TReader reader)
    {
        var value = typeof(ICoreObject).IsAssignableFrom(typeof(T));
        var result = value ? GetCoreObjectList<T>(reader) : GetObjectList<T>(reader);
        return result;
    }
    public virtual List<T> GetObjectList<T>(TReader reader)
    {
        if (!reader.HasRows)
        {
            return new List<T>();
        }
        var result = new List<T>();
        var count = reader.FieldCount;
        var properties = GetFieldProperties<T>(reader);
        while (reader.Read())
        {
            var item = Create<T>();
            for (int i = 0; i < count; i++)
            {
                var prop = properties[i];
                if (prop == null)
                    continue;

                var value = GetValue(reader, i, prop.PropertyType);
                value = IsNullOrDBNull(value) ? null : value;
                prop.SetValue(item, value);
            }
            result.Add(item);
        }
        return result;
    }
    public virtual List<T> GetCoreObjectList<T>(TReader reader)
    {
        if (!reader.HasRows)
        {
            return new List<T>();
        }
        var count = reader.FieldCount;
        var metadata = GetFieldNamesAndProperties<T>(reader);
        var list = new List<ICoreObject>();
        while (reader.Read())
        {
            var item = Create<ICoreObject>(typeof(T));
            item.EnsureCapacity(count);
            for (var i = 0; i < count; i++)
            {
                var (name, prop) = metadata[i];
                var value = prop == null ? GetValue(reader, i) : GetValue(reader, i, prop.PropertyType);
                value = IsNullOrDBNull(value) ? null : value;
                item.Insert(i, name, value);
            }
            list.Add(item);
        }
        var result = list.Cast<T>().ToList();
        return result;
    }

    public virtual CoreDataTable GetDataTable(TReader reader)
    {
        if (!reader.HasRows)
        {
            return new CoreDataTable();
        }
        var count = reader.FieldCount;
        var names = GetFieldNames(reader);
        var result = new CoreDataTable();
        while (reader.Read())
        {
            var item = new CoreDataRow(count);
            for (var i = 0; i < count; i++)
            {
                var value = GetValue(reader, i);
                value = IsNullOrDBNull(value) ? null : value;
                item.Insert(i, names[i], value);
            }
            result.Add(item);
        }
        return result;
    }
    public virtual CoreDataSet GetDataSet(TReader reader)
    {
        var idx = 0;
        var result = new CoreDataSet();
        do
        {
            var item = GetDataTable(reader);
            result.Insert(idx++, $"Table{idx}", item);
        }
        while (reader.NextResult());
        return result;
    }

    #endregion Reader

    #region Execute

    public virtual TReader ExecuteReader(TCommand command)
    {
        var result = (TReader)command.ExecuteReader();
        return result;
    }
    public virtual TReader ExecuteReader(TCommand command, string name)
    {
        throw new NotSupportedException();
    }
    public virtual TReader ExecuteReader(TCommand command, int index)
    {
        throw new NotSupportedException();
    }

    public override int ExecuteNonQuery(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteNonQuery(connection, coreCommand);
        return result;
    }
    public virtual int ExecuteNonQuery(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        var result = command.ExecuteNonQuery();
        return result;
    }

    public override T ExecuteValueFirst<T>(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteValueFirst<T>(connection, coreCommand);
        return result;
    }
    public virtual T ExecuteValueFirst<T>(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command);
        var result = GetValueFirst<T>(reader);
        return result;
    }

    public override List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteValueList<T>(connection, coreCommand);
        return result;
    }
    public virtual List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command);
        var result = GetValueList<T>(reader);
        return result;
    }

    public override List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, int index)
    {
        using var connection = CreateConnection();
        var result = ExecuteValueList<T>(connection, coreCommand, index);
        return result;
    }
    public virtual List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand, int index)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, index);
        var result = GetValueList<T>(reader);
        return result;
    }

    public override List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, string name)
    {
        using var connection = CreateConnection();
        var result = ExecuteValueList<T>(connection, coreCommand, name);
        return result;
    }
    public virtual List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand, string name)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, name);
        var result = GetValueList<T>(reader);
        return result;
    }

    public override List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteRecordList<T>(connection, coreCommand);
        return result;
    }
    public virtual List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command);
        var result = GetRecordList<T>(reader);
        return result;
    }

    public override List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, int index)
    {
        using var connection = CreateConnection();
        var result = ExecuteRecordList<T>(connection, coreCommand, index);
        return result;
    }
    public virtual List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand, int index)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, index);
        var result = GetRecordList<T>(reader);
        return result;
    }

    public override List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, string name)
    {
        using var connection = CreateConnection();
        var result = ExecuteRecordList<T>(connection, coreCommand, name);
        return result;
    }
    public virtual List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand, string name)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, name);
        var result = GetRecordList<T>(reader);
        return result;
    }

    public override CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteDataTable(connection, coreCommand);
        return result;
    }
    public virtual CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command);
        var result = GetDataTable(reader);
        return result;
    }

    public override CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, int index)
    {
        using var connection = CreateConnection();
        var result = ExecuteDataTable(connection, coreCommand, index);
        return result;
    }
    public virtual CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand, int index)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, index);
        var result = GetDataTable(reader);
        return result;
    }

    public override CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, string name)
    {
        using var connection = CreateConnection();
        var result = ExecuteDataTable(connection, coreCommand, name);
        return result;
    }
    public virtual CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand, string name)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command, name);
        var result = GetDataTable(reader);
        return result;
    }

    public override CoreDataSet ExecuteDataSet(ICoreDataCommand coreCommand)
    {
        using var connection = CreateConnection();
        var result = ExecuteDataSet(connection, coreCommand);
        return result;
    }
    public virtual CoreDataSet ExecuteDataSet(TConnection connection, ICoreDataCommand coreCommand)
    {
        using var command = CreateCommand(connection, coreCommand);
        using var reader = ExecuteReader(command);
        var result = GetDataSet(reader);
        return result;
    }

    #endregion Execute
}
