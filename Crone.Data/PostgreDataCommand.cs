using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public record PostgreDataCommand(NpgsqlConnection connection) : DataCommand(connection);
}
