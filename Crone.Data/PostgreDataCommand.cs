namespace Crone
{
	public class PostgreDataCommand : CoreDataCommand
	{
		public PostgreDataCommand(NpgsqlConnection connection) : base(connection) { }
	}
}
