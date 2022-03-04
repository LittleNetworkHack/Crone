using Oracle.ManagedDataAccess.Types;

namespace Crone.Demo
{
	public static class Prototype
	{
		public class PrototypeOraCommand : OracleDataCommand
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

			public OracleDataReader Data
			{
				get => GetProperty<OracleDataReader>();
				set => SetProperty<OracleDataReader>(value);
			}

			public PrototypeOraCommand(OracleConnection connection) : base(connection) { }

			protected override IDbCommand InitializeCommand(IDbConnection connection)
				=> base.InitializeCommand(connection).AsProcedure("INS_AUDIT.TEST.EXAMPLE_PROCEDURE");
		}

		public static void Test()
		{
			
		}

		public class DSH_REPORT : CoreDataRecord
		{
			public int Report_ID
			{
				get => GetProperty<int>();
				set => SetProperty<int>(value);
			}

			public string Title
			{
				get => GetProperty<string>();
				set => SetProperty<string>(value);
			}
		}

		public static void TestOraCommand()
		{
			int count = 0;
			INS_AUDIT.ExamplePackage.ExampleProcedure("Kontakt%", ref count, out var rep1, out var rep2);

		}

		public static void TestDerive()
		{
			using var connection = new OracleConnection(ExampleClass.OracleHRDB);
			using var command = connection.CreateCommand();
			command.AsProcedure("INS_AUDIT.EXAMPLE_PACKAGE.EXAMPLE_PROCEDURE");

			connection.Open();
			OracleCommandBuilder.DeriveParameters(command);
		}

		public static void TestCrone()
		{
			using var connection = new OracleConnection(ExampleClass.OracleHRDB);
			using var command = new PrototypeOraCommand(connection)
			{
				Title = "Kontakt%",
				Count = default,
				Data = default
			};
			command.ExecuteNonQuery();
			using var reader = new CoreDataReader(command.Data);

			var count = command.Count;
			var list = reader.GetRecordList<DSH_REPORT>();
		}

		public static void TestADO()
		{
			using var connection = new OracleConnection(ExampleClass.OracleHRDB);
			using var command = connection.CreateCommand();
			command.AsProcedure("INS_AUDIT.EXAMPLE_PACKAGE.EXAMPLE_PROCEDURE");

			var param0 = command.CreateParameter();
			param0.Value = "Kontakt%";
			command.Parameters.Add(param0);

			var param1 = command.CreateParameter();
			param1.Value = default(int);
			// Nullable bullshit
			command.Parameters.Add(param1);

			var param2 = command.CreateParameter();
			param2.Value = OracleRefCursor.Null;
			command.Parameters.Add(param2);

			connection.Open();
			command.ExecuteNonQuery();

			var count = param1.Value;
			var cursor = param2.Value;
		}


	}
}