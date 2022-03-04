namespace Crone.Demo.Database
{
	public class ExampleSqlCommand : SqlDataCommand
	{
		public string FirstName
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		public ExampleSqlCommand(SqlConnection connection) : base(connection) { }

		protected override IDbCommand InitializeCommand(IDbConnection connection)
			=> base.InitializeCommand(connection).LoadEmbeddedCommand(GetType());
	}
}
