using Oracle.ManagedDataAccess.Types;

namespace Crone.Demo.Database
{
	public class ExampleOraCommand : OracleDataCommand
	{
		public string Title
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		public int Count
		{
			get => GetProperty<int>();
			set => SetProperty<int>(value);
		}

		public ExampleOraCommand(OracleConnection connection) : base(connection) { }

		protected override string GetNameOverride(string name) => $"p_{name}";
		protected override IDbCommand InitializeCommand(IDbConnection connection)
			=> base.InitializeCommand(connection).AsProcedure("INS_AUDIT.TEST.EXAMPLE_PROCEDURE");
	}
}
