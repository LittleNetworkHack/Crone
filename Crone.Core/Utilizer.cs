namespace Crone
{
	public static class Utilizer
	{
		#region Common

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrDBNull(this object value) 
			=> value == null || value == DBNull.Value;

		#endregion Common

		#region String

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullIfEmpty(this string value) 
			=> value == string.Empty ? null : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EmptyIfNull(this string value) 
			=> value == null ? string.Empty : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMinLength(this string value, int min) 
			=> (value?.Length ?? 0) >= min;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMaxLength(this string value, int max) 
			=> (value?.Length ?? 0) <= max;

		#endregion String

		#region Math

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IndexInRange(this int count, int index) 
			=> InRange(index, 0, count - 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InRange<T>(this T value, T min, T max, bool exclusive = false) where T : IComparable<T>
		{
			// x.CompareTo(y):
			//	x > y => 1
			//	x = y => 0
			//  x < y => -1
			int c1 = value.CompareTo(min);
			int c2 = value.CompareTo(max);

			if (c1 == -1 || c1 == 0 && exclusive)
				return false;

			if (c2 == 1 || c2 == 0 && exclusive)
				return false;

			return true;
		}

		public static IEnumerable<decimal> DivideEvenlyAsc(this decimal total, int count, int round = 2) 
			=> DivideEvenly(total, count, round).OrderBy(d => d);
		public static IEnumerable<decimal> DivideEvenlyDesc(this decimal total, int count, int round = 2) 
			=> DivideEvenly(total, count, round).OrderByDescending(d => d);
		public static IEnumerable<decimal> DivideEvenly(this decimal total, int count, int round = 2)
		{
			while (count > 0)
			{
				var value = Math.Round(total / count, round);
				total -= value;
				count -= 1;
				yield return value;
			}
		}

		#endregion Math

		#region Data

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasRows(this DataTable table) => table?.Rows.Count > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataRow FirstRow(this DataTable table) => table.HasRows() ? table.Rows[0] : null;

		#endregion Data

		#region Linq

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach(var item in source)
				action(item);
		}

		public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
		{
			if (!condition)
				return source;

			return source.Where(predicate);
		}

		public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
		{
			if (!condition)
				return source;

			return source.Where(predicate);
		}

		public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, bool>> predicate)
		{
			if (!condition)
				return query;

			return query.Where(predicate);
		}

		public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, int, bool>> predicate)
		{
			if (!condition)
				return query;

			return query.Where(predicate);
		}

		#endregion Linq
	}
}
