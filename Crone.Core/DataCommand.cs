using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public record DataCommand : CoreComponent
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

		public DataCommand(IDbConnection connection) => Command = InitializeCommand(connection);
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
			name = GetNameOverride(name);
			if (Parameters == null)
				return null;

			if (!Parameters.Contains(name))
				return null;

			return (IDataParameter)Parameters[name];
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
			if (parameter == null)
			{
				parameter = Command.CreateParameter();
				Parameters.Insert(index, parameter);
			}
			parameter.Value = value;
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
			if (parameter == null)
			{
				parameter = Command.CreateParameter();
				parameter.ParameterName = name;
				Parameters.Add(parameter);
			}
			parameter.Value = value;
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
