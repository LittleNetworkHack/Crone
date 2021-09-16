using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

		protected virtual IDataReader InitializeReader(IDbCommand command)
		{
			var connection = command.Connection;
			if (connection.State != ConnectionState.Open)
				connection.Open();

			var reader = command.ExecuteReader();
			Count = reader.FieldCount;
			
			Names = new string[Count];
			for (int i = 0; i < Count; i++)
				Names[i] = reader.GetName(i);

			return reader;
		}

		#endregion Constructors

		#region Methods

		public bool Read() => Reader.Read();
		public bool NextResult() => Reader.NextResult();

		public OrderedDictionary GetDictionary()
		{
			var item = new OrderedDictionary(Count);
			for (int i = 0; i < Count; i++)
			{
				var name = Names[i];
				if (string.IsNullOrEmpty(name))
					continue;

				item.Add(name, GetValue(i));
			}
			return item;
		}

		public CoreDataRecord GetRecord()
		{
			var values = GetDictionary();
			return new CoreDataRecord(values);
		}

		public TRecord GetRecord<TRecord>(Func<OrderedDictionary, TRecord> constructor)
			where TRecord : CoreDataRecord
		{
			var values = GetDictionary();
			return constructor(values);
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

		protected override bool SetValueCore(int index, object value) => throw new NotSupportedException();
		protected override bool SetValueCore(string name, object value) => throw new NotSupportedException();

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
