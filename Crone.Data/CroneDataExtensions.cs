using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public static class CroneDataExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IServiceCollection AddSqlConnection(this IServiceCollection services, string connectionString)
			=> services.AddTransient(s => new SqlConnection(connectionString));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IServiceCollection AddOracleConnection(this IServiceCollection services, string connectionString)
			=> services.AddTransient(s => new OracleConnection(connectionString));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IServiceCollection AddPostgreConnection(this IServiceCollection services, string connectionString)
			=> services.AddTransient(s => new NpgsqlConnection(connectionString));
	}
}
