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
	}
}
