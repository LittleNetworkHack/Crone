using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public class PostgreDataCommand : CoreDataCommand
	{
		public PostgreDataCommand(NpgsqlConnection connection) : base(connection) { }
	}
}
