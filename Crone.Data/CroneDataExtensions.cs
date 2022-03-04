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
