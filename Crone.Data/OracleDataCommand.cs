using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public class OracleDataCommand : CoreDataCommand
	{
		public OracleDataCommand(OracleConnection connection) : base(connection) { }
	}
}
