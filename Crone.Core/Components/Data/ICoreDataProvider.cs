namespace Crone;

public interface ICoreDataProvider
{
	string ParameterBinder { get; }
	CoreDataOptions Options { get; }

	string BuildConnection(string server, int port, string database, string username, string password, string schema);

	int ExecuteNonQuery(ICoreDataCommand coreCommand);
	CoreDataParameters ExecuteProcedure(ICoreDataCommand coreCommand);

	CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand);
	CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, int index);
	CoreDataTable ExecuteDataTable(ICoreDataCommand coreCommand, string name);
	CoreDataSet ExecuteDataSet(ICoreDataCommand coreCommand);

	T ExecuteValueFirst<T>(ICoreDataCommand coreCommand);
	List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand);
	List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, int index);
	List<T> ExecuteValueList<T>(ICoreDataCommand coreCommand, string name);
	List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand);
	List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, int index);
	List<T> ExecuteRecordList<T>(ICoreDataCommand coreCommand, string name);
}

public interface ICoreDataProvider<TConnection, TCommand, TReader> : ICoreDataProvider
	where TConnection : DbConnection
	where TCommand : DbCommand
	where TReader : DbDataReader
{
	#region Connection

	TConnection CreateConnection();
	TConnection CreateConnection(CoreDataOptions options);
	TConnection CreateConnection(string server, int port, string database, string username, string password, string schema);
	TConnection CreateConnection(string connectionString);

	#endregion Connection

	#region Command

	TCommand CreateCommand(TConnection connection);
	TCommand CreateCommand(TConnection connection, ICoreDataCommand command);

	TCommand DeriveCommandParameters(TCommand command);
	TCommand BindCommandByName(TCommand command, CoreDataParameters parameters, ParameterDirection? direction = null);
	TCommand BindCommandByPosition(TCommand command, CoreDataParameters parameters, ParameterDirection? direction = null);

	T GetParameter<T>(TCommand command, int index);
	void SetParameter(TCommand command, int index, object value, ParameterDirection? direction = null);

	T GetParameter<T>(TCommand command, string name);
	void SetParameter(TCommand command, string name, object value, ParameterDirection? direction = null);

	#endregion Command

	#region Reader

	T GetValue<T>(TReader reader, int index);
	object GetValue(TReader reader, int index);
	object GetValue(TReader reader, int index, Type type);

	T GetValue<T>(TReader reader, string name);
	object GetValue(TReader reader, string name);
	object GetValue(TReader reader, string name, Type type);

	T GetValueFirst<T>(TReader reader);
	List<T> GetValueList<T>(TReader reader);

	string[] GetFieldNames(TReader reader);
	PropertyInfo[] GetFieldProperties<T>(TReader reader);
	CoreDataReaderColumn[] GetFieldNamesAndProperties<T>(TReader reader);

	List<T> GetObjectList<T>(TReader reader);
	List<T> GetRecordList<T>(TReader reader);
	List<T> GetCoreObjectList<T>(TReader reader);

	CoreDataTable GetDataTable(TReader reader);
	CoreDataSet GetDataSet(TReader reader);

	#endregion Reader

	#region Execute

	TReader ExecuteReader(TCommand command);
	TReader ExecuteReader(TCommand command, int index);
	TReader ExecuteReader(TCommand command, string name);

	int ExecuteNonQuery(TConnection connection, ICoreDataCommand coreCommand);
	CoreDataParameters ExecuteProcedure(TConnection connection, ICoreDataCommand coreCommand);

	CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand);
	CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand, int index);
	CoreDataTable ExecuteDataTable(TConnection connection, ICoreDataCommand coreCommand, string name);
	CoreDataSet ExecuteDataSet(TConnection connection, ICoreDataCommand coreCommand);

	T ExecuteValueFirst<T>(TConnection connection, ICoreDataCommand coreCommand);
	List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand);
	List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand, int index);
	List<T> ExecuteValueList<T>(TConnection connection, ICoreDataCommand coreCommand, string name);

	List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand);
	List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand, int index);
	List<T> ExecuteRecordList<T>(TConnection connection, ICoreDataCommand coreCommand, string name);

	#endregion Execute
}
