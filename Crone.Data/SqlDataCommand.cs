using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public class SqlDataCommand : CoreDataCommand
	{
		public SqlDataCommand(SqlConnection connection) : base(connection) { }

		protected override string GetNameOverride(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;

			if (name.StartsWith('@'))
				return name;

			return "@" + name;
		}
	}
}
