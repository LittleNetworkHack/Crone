using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Crone
{
	public class Program
	{
		public static readonly string AdventureDB = "Server=localhost;Database=AdventureWorks2019;Trusted_Connection=True;";

		static void Main()
		{
#if RELEASE
			BenchmarkRunner.Run<Benchy>();
#else
			Test();
#endif
		}

		static void Test()
		{
			Benchy b = new Benchy();
		}
	}

	public record PersonCommand(SqlConnection connection) : SqlDataCommand(connection)
	{
		public string Name
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		protected override IDbCommand InitializeCommand(IDbConnection connection)
		{
			var cmd = base.InitializeCommand(connection);
			cmd.PresetCommand("sp_Test_Person", CommandType.StoredProcedure);
			return cmd;
		}
	}

	public record PersonRecord : DataRecord
	{
		public PersonRecord() : base() { }
		public PersonRecord(OrderedDictionary properties) : base(properties) { } 

		public int? BusinessEntityID
		{
			get => GetProperty<int?>();
			set => SetProperty<int?>(value);
		}
		public string FirstName
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}
	}

	[MemoryDiagnoser]
	public class Benchy
	{
		static Benchy() { }

		#region Helpers

		public IPropertyKey[] GetKeys(IDataReader reader, Type type)
		{
			var props = PropertyKey.GetKeys(type);
			IPropertyKey[] keys = new IPropertyKey[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
			{
				string name = reader.GetName(i);
				keys[i] = props.TryGetValue(name, out var key) ? key : null;
			}
			return keys;
		}

		public PropertyInfo[] GetInfos(SqlDataReader reader, Type type)
		{
			var props = type.GetProperties();
			PropertyInfo[] keys = new PropertyInfo[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
			{
				string name = reader.GetName(i);
				keys[i] = props.FirstOrDefault(p => p.Name == name);
			}
			return keys;
		}

		public string[] GetNames(IDataReader reader)
		{
			string[] names = new string[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
				names[i] = reader.GetName(i);

			return names;
		}

		public SqlConnection GetPersonConnection()
		{
			return new SqlConnection(Program.AdventureDB);
		}

		public PersonCommand GetPersonCommand(SqlConnection connection, string name)
		{
			return new PersonCommand(connection)
			{
				Name = name
			};
		}

		#endregion Helpers
	}
}
