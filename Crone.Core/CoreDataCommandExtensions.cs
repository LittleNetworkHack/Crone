namespace Crone
{
	public static class CoreDataCommandExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T PresetCommand<T>(this T command, string text, CommandType type = CommandType.Text) where T : CoreDataCommand
		{
			command.CommandText = text;
			command.CommandType = type;
			return command;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T PresetDbCommand<T>(this T command, string text, CommandType type = CommandType.Text) where T : class, IDbCommand
		{
			command.CommandText = text;
			command.CommandType = type;
			return command;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T AsQuery<T>(this T command, string commandText)
			where T : class, IDbCommand
		{
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;
			return command;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T AsProcedure<T>(this T command, string commandText)
			where T : class, IDbCommand
		{
			command.CommandText = commandText;
			command.CommandType = CommandType.StoredProcedure;
			return command;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T LoadEmbeddedCommand<T>(this T command, Type sourceType) where T : class, IDbCommand
		{
			using var stream = sourceType.Assembly.GetManifestResourceStream(sourceType.FullName);
			using var reader = new StreamReader(stream);
			var text = reader.ReadToEnd();
			return PresetDbCommand(command, text, CommandType.Text);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ExecuteNonQuery<T>(this T command) where T : CoreDataCommand
		{
			var connection = command.Connection;
			if (connection.State != ConnectionState.Open)
				connection.Open();

			return command.Command.ExecuteNonQuery();
		}
	}
}
