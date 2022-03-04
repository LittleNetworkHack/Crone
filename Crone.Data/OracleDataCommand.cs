using Oracle.ManagedDataAccess.Types;

namespace Crone
{
	public class OracleDataCommand : CoreDataCommand
	{
		public OracleDataCommand(OracleConnection connection) : base(connection) { }

		protected override IDbCommand InitializeCommand(IDbConnection connection)
		{
			var command = (OracleCommand)base.InitializeCommand(connection);
			command.BindByName = true;
			return command;
		}

		protected override T GetProperty<T>([CallerMemberName] string name = null)
		{
			if (typeof(T) == typeof(OracleDataReader))
			{
				var value = GetValue(name);
				return value is T reader ? reader : default(T);
			}

			return base.GetProperty<T>(name);
		}

		protected override void SetProperty<T>(object value, [CallerMemberName] string name = null)
		{
			if (typeof(T) == typeof(OracleDataReader))
			{
				SetValue(name, OracleRefCursor.Null);
				return;
			}
			base.SetProperty<T>(value, name);
		}

		protected override string GetNameOverride(string name) => $"p_{name}";
	}
}
