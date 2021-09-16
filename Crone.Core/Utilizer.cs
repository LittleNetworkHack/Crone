using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public static class Utilizer
	{
		#region Common

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrDBNull(this object value) 
			=> value == null || value == DBNull.Value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOneOf<T>(this T value, params T[] values)
			=> values.Any(v => EqualityComparer<T>.Default.Equals(value, v));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryExecute(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryExecute<TArg>(Action<TArg> action, TArg value)
		{
			try
			{
				action(value);
				return true;
			}
			catch
			{
				return false;
			}
		}

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
				decimal amount = Math.Round(total / count, round);
				total -= amount;
				count--;
				yield return amount;
			}
		}

		#endregion Math

		#region Component

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
		public static void InvokeEmpty(this IComponent component, EventHandler handler)
			=> handler?.Invoke(component, EventArgs.Empty);

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

		#endregion Component

		#region Data

		public static DataRow FirstRow(this DataTable table)
		{
			if (table == null || table.Rows.Count == 0)
				return null;

			return table.Rows[0];
		}

		public static bool HasRows(this DataTable table) => table?.Rows.Count > 0;

		#endregion Data
	}
}
