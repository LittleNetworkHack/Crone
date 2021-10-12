using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone.Demo.Database
{
	public class ExampleOraCommand : OracleDataCommand
	{
		public decimal Salary
		{
			get => GetProperty<decimal>();
			set => SetProperty<decimal>(value);
		}

		public string FirstName
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		public ExampleOraCommand(OracleConnection connection) : base(connection) { }

		protected override IDbCommand InitializeCommand(IDbConnection connection)
			=> base.InitializeCommand(connection).LoadEmbeddedCommand(GetType());
	}
}
