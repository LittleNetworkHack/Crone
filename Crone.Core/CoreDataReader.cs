namespace Crone
{
	public class CoreDataReader : CoreComponent
	{

		#region Properties

		public int Count { get; protected set; }
		public string[] Names { get; protected set; }

		public IDataReader Reader { get; protected set; }

		#endregion Properties

		#region Constructors

		public CoreDataReader(CoreDataCommand command) => Reader = InitializeReader(command.Command);
		public CoreDataReader(IDbCommand command) => Reader = InitializeReader(command);
		public CoreDataReader(IDataReader reader) => Reader = InitializeReader(reader);

		protected virtual IDataReader InitializeReader(IDbCommand command)
		{
			var connection = command.Connection;
			if (connection.State != ConnectionState.Open)
				connection.Open();

			var reader = command.ExecuteReader();
			return InitializeReader(reader);
		}

		protected virtual IDataReader InitializeReader(IDataReader reader)
		{
			Count = reader.FieldCount;
			Names = new string[Count];
			for (int i = 0; i < Count; i++)
				Names[i] = reader.GetName(i);

			return reader;
		}

		#endregion Constructors

		#region Methods

		public bool Read() => Reader.Read();
		public bool NextResult()
		{
			if (!Reader.NextResult())
				return false;

			InitializeReader(Reader);
			return true;
		}

		#endregion Methods

		#region Get/Set Core

		protected override bool GetValueCore(int index, out object value)
		{
			try
			{
				value = Reader[index];
				return true;
			}
			catch
			{
				value = null;
				return false;
			}
		}

		protected override bool GetValueCore(string name, out object value)
		{
			try
			{
				value = Reader[name];
				return true;
			}
			catch
			{
				value = null;
				return false;
			}
		}

		const string err_cant_set_val = "IDataReader is read-only!";
		protected override bool SetValueCore(int index, object value) => throw new NotSupportedException(err_cant_set_val);
		protected override bool SetValueCore(string name, object value) => throw new NotSupportedException(err_cant_set_val);

		#endregion Get/Set Core

		#region IDisposable
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			Reader.TryDispose();
			base.Dispose(disposing);
		}
		#endregion IDisposable
	}
}
