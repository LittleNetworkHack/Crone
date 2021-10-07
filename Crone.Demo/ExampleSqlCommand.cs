using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone.Demo.Database
{
	public class ExampleSqlCommand : SqlDataCommand
	{
		public string FirstName
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		public ExampleSqlCommand(SqlConnection connection) : base(connection) { }

		protected override IDbCommand InitializeCommand(IDbConnection connection)
			=> base.InitializeCommand(connection).LoadEmbeddedCommand(GetType());
	}
}
