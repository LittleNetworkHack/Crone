using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public static class ExampleClass
	{
		public static readonly string OracleHRDB = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=HR;Password=hr;";
		public static readonly string AdventureDB = "Server=localhost;Database=AdventureWorks2019;User Id=aw;Password=aw;";
		

		public static void TestAll()
		{
			ConverterExample();
			MathExamples();
			PropertyKeyExample();
			ObjectActivatorExample();
			DataRecordExample();
			DataCommandExample();
		}

		// See ValueConverter.cs for rules
		public static void ConverterExample()
		{
			int i = 5;
			bool f = true;
			string s = "aaa";


			int x1 = ValueConverter.ConvertTo<int>(f); // Should be: 1
			int? x2 = ValueConverter.ConvertTo<int?>(s); // Should be: null
			bool x3 = ValueConverter.ConvertTo<bool>(i); // Should be: true

			object text = "01/01/2020";
			Type type = typeof(DateTime);
			object something = ValueConverter.ConvertTo(text, type, DateTime.Now); // Convert unknown to type variable with default value


			decimal d = 123.456M;
			var db = ValueConverter.ToByteArray(d);
			var e = ValueConverter.ToDecimal(db);
		}

		// See Utilizer.cs for more
		public static void MathExamples()
		{
			bool flag;
			List<int> list = Enumerable.Range(1, 10).ToList();

			flag = list.Count.IndexInRange(2); // Should be: true
			flag = list.Count.IndexInRange(20); // Should be: false

			decimal amount = 26M; // Some amount
			int count = 6; // Value that causes problem while dividing
			var result = amount.DivideEvenly(count).ToArray(); // Spreads 0.01 reminder pseudo randomly among results (used for currency, rounded to 2 decimal places!)
															   // Should be: 4.33, 4.33, 4.34, 4.33, 4.34, 4.33
		}

		public record PlainRecord
		{
			public string Name { get; set; }
			public int? Age { get; set; }

			public PlainRecord() { }
			public PlainRecord(string Name, int? Age)
			{
				this.Name = Name;
				this.Age = Age;
			}
		}

		// See PropertyKey.cs if intrested in implementation (not recommended)
		public static void PropertyKeyExample()
		{
			var prk = PropertyKey.GetKeys<PlainRecord>();
			var nameKey = prk["Name"];
			var pr = new PlainRecord("aaa", 20);
			nameKey.SetBoxedValue(pr, "ccc");

			string name = pr.Name; // Should be: ccc
		}

		// ObjectActivator is cached replacement for Activator.CreateInstance()
		public static void ObjectActivatorExample()
		{
			object obj = ObjectActivator.Create(typeof(PlainRecord)); // Creates instance of type with object result
			PlainRecord pr = ObjectActivator.Create<PlainRecord>(); // Creates instance of generic type with generic result

			IConvertible x = ObjectActivator.CreateAs<IConvertible>(typeof(int)); // Creates instance of type with casted result
			IList y = ObjectActivator.CreateAs<IList>(typeof(List<>), typeof(int)); // Creates instance List<int> with casted result
		}

		// DataRecord that lets us define properties using strong or dynamic name
		public class PersonRecord : CoreDataRecord
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

		// Inherit constructor for DI
		// See other types in DataCommand.cs
		public class PersonCommand : SqlDataCommand
		{
			public PersonCommand(SqlConnection connection) : base(connection) { }

			// Compile time inferred name using CallerMemberName
			public string FirstName
			{
				get => GetProperty<string>();
				set => SetProperty<string>(value);
			}

			// Preset query and type
			// base calls CreateCommand() and is provider agnostic (has predefined type like SqlDataCommand for DI)
			protected override IDbCommand InitializeCommand(IDbConnection connection)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("SELECT P.*");
				sb.AppendLine("FROM Person.Person P");
				sb.AppendLine("WHERE @FirstName IS NULL OR P.FirstName LIKE @FirstName");

				// Use fluid chain 
				return base.InitializeCommand(connection).PresetDbCommand(sb.ToString(), CommandType.Text);
				// For stored procedure:
				//return base.InitializeCommand(connection).PresetCommand("sp_Test_Person", CommandType.StoredProcedure);
			}
		}

		// Example of mixing strong/dynamic names and indexes for get/set value
		public static void DataRecordExample()
		{
			var ex = new PersonRecord();
			ex.SetValue("FirstName", "test"); // Index 0
			ex.BusinessEntityID = 50; // Index 1

			string name = ex.FirstName; // Should be: test
			decimal? id = ex.GetValue<decimal?>("BusinessEntityID"); // GetValue with conversion


			ex.SetValue(1, 5); // Set age (index 1), used only if key BusinessEntityID already exists
							   // Order is random, useful only for table like access or in for/foreach scenario
							   // If we wrote:
							   //	ex.BusinessEntityID = 50;
							   //	ex.SetValue("FirstName", "test");
							   // Then BusinessEntityID index would be: 0

			var x = ex.BusinessEntityID; // Should be: 5
		}

		// Use services for DI in DataCommand.cs (DataCommandEx) ex:
		//		IServiceCollection services = null;
		//		services.AddSqlConnection(connStr);
		public static void DataCommandExample()
		{
			using var connection = new SqlConnection(AdventureDB);
			using var command = new PersonCommand(connection)
			{
				FirstName = "Am%"
			};
			using var reader = new CoreDataReader(command);

			var list = new List<PersonRecord>();
			while (reader.Read())
			{
				var item = reader.GetRecord(v => new PersonRecord(v));
				list.Add(item);
			}


			// list should contain 125 persons where names starts with Am
			//return list;
		}

		public static void DataTableExample()
		{
			using var connection = new SqlConnection(AdventureDB);
			using var command = new PersonCommand(connection)
			{
				FirstName = "Am%"
			};
			using var adapter = new SqlDataAdapter((SqlCommand)command.Command);
			var table = new DataTable();
			adapter.Fill(table);

			var list = new List<PersonRecord>();


			// list should contain 125 persons where names starts with Am
			//return list;
		}
	}
}
