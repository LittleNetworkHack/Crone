namespace Crone
{
	public static class CoreComponentExtensions
    {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TryDispose(this object value)
			=> TryDispose(value as IDisposable);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TryDispose(this IDisposable value)
		{
			try
			{
				value?.Dispose();
			}
			catch { }
		}
	}
}
