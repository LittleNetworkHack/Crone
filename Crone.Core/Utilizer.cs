using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

		public static byte[] GetMD5Hash(string value)
		{
			using var hasher = MD5.Create();
			var bytes = Encoding.UTF8.GetBytes(value);
			return hasher.ComputeHash(bytes);
		}

		public static byte[] GetMD5Hash(Stream stream)
		{
			using var hasher = MD5.Create();
			return hasher.ComputeHash(stream);
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

		public static T LoadEmbeddedCommand<T>(this T command, Type sourceType) where T : class, IDbCommand
		{
			using var stream = sourceType.Assembly.GetManifestResourceStream(sourceType.FullName);
			using var reader = new StreamReader(stream);
			var text = reader.ReadToEnd();
			return PresetDbCommand(command, text, CommandType.Text);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasRows(this DataTable table) => table?.Rows.Count > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataRow FirstRow(this DataTable table)
		{
			if (!table.HasRows())
				return null;

			return table.Rows[0];
		}

		#endregion Data

		#region JsonSerialization

		public static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions()
		{
			Converters =
			{
				new CroneDBNullConverter()
			}
		};

		private class CroneDBNullConverter : JsonConverter<DBNull>
		{
			public override DBNull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
				=> reader.TokenType == JsonTokenType.Null ? DBNull.Value : throw new JsonException("Fail to deserialize DBNull value!");

			public override void Write(Utf8JsonWriter writer, DBNull value, JsonSerializerOptions options)
				=> writer.WriteNullValue();
		}

		#endregion JsonSerialization
	}
}
