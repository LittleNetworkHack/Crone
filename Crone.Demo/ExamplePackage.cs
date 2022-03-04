using Crone;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INS_AUDIT
{
	public class DSH_REPORT : CoreDataRecord
	{
		public DSH_REPORT()
		{
		}

		public DSH_REPORT(OrderedDictionary properties) : base(properties)
		{
		}

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

	public abstract class OraclePackage
	{
		public abstract string Schema { get; }
		public abstract string Package { get; }
		public abstract string ConnectionString { get; }

	}

	public static class ExamplePackage
	{
		private static readonly string SCHEMA = "INS_AUDIT";
		private static readonly string PACKAGE = "EXAMPLE_PACKAGE";

		private static OracleConnection CreateConnection()
		{
			var connection = new OracleConnection(ExampleClass.OracleHRDB);
			connection.Open();
			return connection;
		}
		private static OracleCommand CreateProcedure(this OracleConnection connection, string procedure)
		{
			var command = new OracleCommand();
			command.Connection = connection;
			command.CommandText = $"{SCHEMA}.{PACKAGE}.{procedure}";
			command.CommandType = CommandType.StoredProcedure;
			OracleCommandBuilder.DeriveParameters(command);
			return command;
		}
		private static T GetParameter<T>(this OracleCommand command, int index)
		{
			var value = command.Parameters[index].Value;
			return ValueConverter.ConvertTo<T>(value);
		}

		private static IEnumerable<OrderedDictionary> AsEnumerable(this OracleDataReader reader)
		{
			var count = reader.FieldCount;
			var names = new string[count];
			for (int i = 0; i < count; i++)
				names[i] = reader.GetName(i);

			while (reader.Read())
			{
				var record = new OrderedDictionary(count);
				for (int i = 0; i < count; i++)
				{
					var value = reader.GetValue(i);
					record.Insert(i, names[i], value);
				}
				yield return record;
			}
		}

		private static OrderedDictionary[] ReadArray(this OracleDataReader reader) => reader.AsEnumerable().ToArray();

		public static void ExampleProcedure(string title, ref int count, out OrderedDictionary[] rep1, out DSH_REPORT[] rep2)
		{
			using var connection = CreateConnection();
			using var command = connection.CreateProcedure("EXAMPLE_PROCEDURE");

			var args = command.Parameters;
			args[0].Value = title;
			args[1].Value = count;

			using var reader = command.ExecuteReader();
			count = command.GetParameter<int>(1);
			rep1 = reader.ReadArray();
			reader.NextResult();
			rep2 = reader.AsEnumerable().Select(e => new DSH_REPORT(e)).ToArray();
		}
	}
}
