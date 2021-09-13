﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public record OracleDataCommand(OracleConnection connection) : DataCommand(connection);
}
