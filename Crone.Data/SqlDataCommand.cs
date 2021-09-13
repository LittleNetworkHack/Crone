using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public record SqlDataCommand(SqlConnection connection) : DataCommand(connection)
	{
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
