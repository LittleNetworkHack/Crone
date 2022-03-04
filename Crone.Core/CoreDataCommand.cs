namespace Crone
{
	public class CoreDataCommand : CoreComponent
	{
		#region Properties

		public IDbCommand Command { get; protected set; }
		public string CommandText
		{
			get => Command.CommandText;
			set => Command.CommandText = value;
		}
		public CommandType CommandType
		{
			get => Command.CommandType;
			set => Command.CommandType = value;
		}
		public IDbConnection Connection => Command?.Connection;
		public IDataParameterCollection Parameters => Command?.Parameters;

		#endregion Properties

		#region Constructors

		public CoreDataCommand(IDbConnection connection) => Command = InitializeCommand(connection);
		protected virtual IDbCommand InitializeCommand(IDbConnection connection) => connection.CreateCommand();

		#endregion Constructors

		#region Parameter

		protected IDataParameter ResolveParameter(int index)
		{
			if (Parameters == null)
				return null;

			if (!Parameters.Count.IndexInRange(index))
				return null;

			return (IDataParameter)Parameters[index];
		}
		protected IDataParameter ResolveParameter(string name)
		{
			if (Parameters == null)
				return null;

			if (!Parameters.Contains(name))
				return null;

			return (IDataParameter)Parameters[name];
		}

		protected virtual IDataParameter CreateParameter(string name, object value)
		{
			var parameter = Command.CreateParameter();
			parameter.ParameterName = name.EmptyIfNull();
			parameter.Value = value;
			return parameter;
		}

		#endregion Parameter

		#region Get/Set Core
		protected override bool GetValueCore(int index, out object value)
		{
			value = null;
			var parameter = ResolveParameter(index);
			if (parameter == null)
				return false;

			value = parameter.Value;
			return true;
		}
		protected override bool SetValueCore(int index, object value)
		{
			if (Parameters == null)
				return false;

			var parameter = ResolveParameter(index);
			if (parameter != null)
			{
				parameter.Value = value;
				return true;
			}
			
			parameter = CreateParameter(null, value);
			Parameters.Insert(index, parameter);
			return true;
		}
		protected override bool GetValueCore(string name, out object value)
		{
			value = null;
			var parameter = ResolveParameter(name);
			if (parameter == null)
				return false;

			value = parameter.Value;
			return true;
		}
		protected override bool SetValueCore(string name, object value)
		{
			if (Parameters == null)
				return false;

			var parameter = ResolveParameter(name);
			if (parameter != null)
			{
				parameter.Value = value;
				return true;
			}

			parameter = CreateParameter(name, value);
			Parameters.Add(parameter);
			return true;
		}
		#endregion Get/Set Core

		#region IDisposable
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			Command.TryDispose();
			base.Dispose(disposing);
		}
		#endregion IDisposable
	}
}
