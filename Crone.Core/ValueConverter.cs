using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Crone
{
	public static partial class ValueConverter
	{
		#region Helpers

		private static readonly Type[] Primitives = new Type[]
		{
			typeof(bool),
			typeof(char),
			typeof(string),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(byte),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(byte[]),
			typeof(DateTime),
			typeof(Guid)
		};

		public static bool IsPrimitive(Type type) => type.IsEnum || Primitives.Contains(type) || Primitives.Contains(Nullable.GetUnderlyingType(type));

		public static TValue? TryCast<TValue>(object value)
			where TValue : struct
		{
			try
			{
				return (TValue)value;
			}
			catch
			{
				return null;
			}
		}

		#endregion Helpers

		#region ConvertTo

		public static T ConvertTo<T>(object value)
		{
			if (value is T casted)
				return casted;

			return (T)ConvertTo(value, typeof(T));
		}

		public static object ConvertTo(object value, Type destType, object defaultValue)
		{
			if (value.IsNullOrDBNull())
				return defaultValue;

			if (destType == typeof(object))
				return value;

			Type srcType = value.GetType();
			if (srcType == destType)
				return value;

			destType = Nullable.GetUnderlyingType(destType) ?? destType;

			Type enumType = srcType.IsEnum ? Enum.GetUnderlyingType(srcType) : null;
			if (enumType != null && enumType != destType)
				value = ConvertTo(value, enumType);

			if (destType.IsEnum)
				value = ToEnum(value, destType);
			else if (destType == typeof(bool))
				value = ToBoolean(value);
			else if (destType == typeof(char))
				value = ToChar(value);
			else if (destType == typeof(string))
				value = ToString(value);

			else if (destType == typeof(float))
				value = ToSingle(value);
			else if (destType == typeof(double))
				value = ToDouble(value);
			else if (destType == typeof(decimal))
				value = ToDecimal(value);

			else if (destType == typeof(byte))
				value = ToByte(value);
			else if (destType == typeof(short))
				value = ToInt16(value);
			else if (destType == typeof(int))
				value = ToInt32(value);
			else if (destType == typeof(long))
				value = ToInt64(value);

			else if (destType == typeof(sbyte))
				value = ToSByte(value);
			else if (destType == typeof(ushort))
				value = ToUInt16(value);
			else if (destType == typeof(uint))
				value = ToUInt32(value);
			else if (destType == typeof(ulong))
				value = ToUInt64(value);

			else if (destType == typeof(byte[]))
				value = ToByteArray(value);
			else if (destType == typeof(DateTime))
				value = ToDateTime(value);
			//else if (destType == typeof(TimeSpan))
			//	value = ToUInt32(value);
			else if (destType == typeof(Guid))
				value = ToGuid(value);

			return value ?? defaultValue;
		}

		public static object ConvertTo(object value, Type destType)
		{
			object defaultValue = ObjectActivator.GetDefaultOrNull(destType);
			return ConvertTo(value, destType, defaultValue);
		}

		public static IEnumerable<T> ConvertAll<T>(this IEnumerable values)
		{
			foreach (object item in values)
				yield return ConvertTo<T>(item);
		}

		#endregion ConvertTo

		#region OutOfRange

		public static bool OutOfRangeFloat(float value, float min, float max) => value < min || value > max;
		public static bool OutOfRangeDouble(double value, double min, double max) => value < min || value > max;
		public static bool OutOfRangeDecimal(decimal value, decimal min, decimal max) => value < min || value > max;
		public static bool OutOfRangeBinary(byte[] value, int size)
		{
			if (value == null)
				return true;

			if (size == 0)
				return false;

			return value.Length != size;
		}

		#endregion OutOfRange

		#region Binary

		public static byte[] ToByteArray(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToByteArray(v);
				case char v:
					return ToByteArray(v);
				case string v:
					return ToByteArray(v);

				case float v:
					return ToByteArray(v);
				case double v:
					return ToByteArray(v);
				case decimal v:
					return ToByteArray(v);

				case byte v:
					return ToByteArray(v);
				case short v:
					return ToByteArray(v);
				case int v:
					return ToByteArray(v);
				case long v:
					return ToByteArray(v);

				case sbyte v:
					return ToByteArray(v);
				case ushort v:
					return ToByteArray(v);
				case uint v:
					return ToByteArray(v);
				case ulong v:
					return ToByteArray(v);

				case byte[] v:
					return ToByteArray(v);
				case DateTime v:
					return ToByteArray(v);
				//case TimeSpan v:
				//	return ToByteArray(v);
				//case XmlDocument v:
				//	return ToByteArray(v);
				case Guid v:
					return ToByteArray(v);
				default:
					return null;
			}
		}
		public static byte[] ToByteArray(bool value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(char value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(string value) => value != null ? Encoding.UTF8.GetBytes(value) : null;

		public static byte[] ToByteArray(byte value) => new byte[] { value };
		public static byte[] ToByteArray(short value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(int value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(long value) => BitConverter.GetBytes(value);

		public static byte[] ToByteArray(sbyte value) => new byte[] { (byte)value };
		public static byte[] ToByteArray(ushort value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(uint value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(ulong value) => BitConverter.GetBytes(value);

		public static byte[] ToByteArray(float value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(double value) => BitConverter.GetBytes(value);
		public static byte[] ToByteArray(decimal value)
		{
			try
			{
				var bits = decimal.GetBits(value);
				var a = ToByteArray(bits[0]);
				var b = ToByteArray(bits[1]);
				var c = ToByteArray(bits[2]);
				var d = ToByteArray(bits[3]);
				return a.Concat(b).Concat(c).Concat(d).ToArray();
			}
			catch
			{
				return null;
			}
		}

		public static byte[] ToByteArray(byte[] value) => value;

		public static byte[] ToByteArray(DateTime value) => BitConverter.GetBytes(value.Ticks);
		public static byte[] ToByteArray(Guid value) => value.ToByteArray();

		#endregion Binary

		#region Boolean

		public static bool? ToBoolean(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToBoolean(v);
				case char v:
					return ToBoolean(v);
				case string v:
					return ToBoolean(v);

				case float v:
					return ToBoolean(v);
				case double v:
					return ToBoolean(v);
				case decimal v:
					return ToBoolean(v);

				case byte v:
					return ToBoolean(v);
				case short v:
					return ToBoolean(v);
				case int v:
					return ToBoolean(v);
				case long v:
					return ToBoolean(v);

				case sbyte v:
					return ToBoolean(v);
				case ushort v:
					return ToBoolean(v);
				case uint v:
					return ToBoolean(v);
				case ulong v:
					return ToBoolean(v);

				case byte[] v:
					return ToBoolean(v);
				//case DateTime v:
				//	return ToBoolean(v);
				//case TimeSpan v:
				//	return ToBoolean(v);
				//case XmlDocument v:
				//	return ToBoolean(v);
				//case Guid v:
				//	return ToBoolean(v);
				default:
					return TryCast<bool>(value);
			}
		}

		public static bool? ToBoolean(bool value) => value;
		public static bool? ToBoolean(char value) => value != '\0' ? true : false;
		public static bool? ToBoolean(string value) => bool.TryParse(value, out bool result) ? (bool?)result : null;

		public static bool? ToBoolean(byte value) => value != 0 ? true : false;
		public static bool? ToBoolean(short value) => value != 0 ? true : false;
		public static bool? ToBoolean(int value) => value != 0 ? true : false;
		public static bool? ToBoolean(long value) => value != 0 ? true : false;

		public static bool? ToBoolean(sbyte value) => value != 0 ? true : false;
		public static bool? ToBoolean(ushort value) => value != 0 ? true : false;
		public static bool? ToBoolean(uint value) => value != 0 ? true : false;
		public static bool? ToBoolean(ulong value) => value != 0 ? true : false;

		public static bool? ToBoolean(float value) => value != 0 ? true : false;
		public static bool? ToBoolean(double value) => value != 0 ? true : false;
		public static bool? ToBoolean(decimal value) => value != 0 ? true : false;

		public static bool? ToBoolean(byte[] value) => OutOfRangeBinary(value, sizeof(bool)) ? null : (bool?)BitConverter.ToBoolean(value, 0);
		#endregion Boolean

		#region Byte

		public static byte? ToByte(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToByte(v);
				case char v:
					return ToByte(v);
				case string v:
					return ToByte(v);

				case float v:
					return ToByte(v);
				case double v:
					return ToByte(v);
				case decimal v:
					return ToByte(v);

				case byte v:
					return ToByte(v);
				case short v:
					return ToByte(v);
				case int v:
					return ToByte(v);
				case long v:
					return ToByte(v);

				case sbyte v:
					return ToByte(v);
				case ushort v:
					return ToByte(v);
				case uint v:
					return ToByte(v);
				case ulong v:
					return ToByte(v);

				case byte[] v:
					return ToByte(v);
				//case DateTime v:
				//	return ToByte(v);
				//case TimeSpan v:
				//	return ToByte(v);
				//case XmlDocument v:
				//	return ToByte(v);
				//case Guid v:
				//	return ToByte(v);
				default:
					return TryCast<byte>(value);
			}
		}

		public static byte? ToByte(bool value) => (byte?)(value ? 1 : 0);
		public static byte? ToByte(char value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(string value) => byte.TryParse(value, out byte result) ? (byte?)result : null;

		public static byte? ToByte(byte value) => value;
		public static byte? ToByte(short value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(int value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(long value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;

		public static byte? ToByte(sbyte value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(ushort value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(uint value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(ulong value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;

		public static byte? ToByte(float value) => OutOfRangeFloat(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(double value) => OutOfRangeDouble(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;
		public static byte? ToByte(decimal value) => OutOfRangeDecimal(value, byte.MinValue, byte.MaxValue) ? null : (byte?)value;

		public static byte? ToByte(byte[] value) => OutOfRangeBinary(value, sizeof(byte)) ? null : (byte?)value[0];

		#endregion Byte

		#region Char

		public static char? ToChar(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToChar(v);
				case char v:
					return ToChar(v);
				case string v:
					return ToChar(v);

				case float v:
					return ToChar(v);
				case double v:
					return ToChar(v);
				case decimal v:
					return ToChar(v);

				case byte v:
					return ToChar(v);
				case short v:
					return ToChar(v);
				case int v:
					return ToChar(v);
				case long v:
					return ToChar(v);

				case sbyte v:
					return ToChar(v);
				case ushort v:
					return ToChar(v);
				case uint v:
					return ToChar(v);
				case ulong v:
					return ToChar(v);

				case byte[] v:
					return ToChar(v);
				//case DateTime v:
				//	return ToChar(v);
				//case TimeSpan v:
				//	return ToChar(v);
				//case XmlDocument v:
				//	return ToChar(v);
				//case Guid v:
				//	return ToChar(v);
				default:
					return TryCast<char>(value);
			}
		}

		public static char? ToChar(bool value) => value ? '\u0001' : '\u0000';
		public static char? ToChar(char value) => value;
		public static char? ToChar(string value) => string.IsNullOrEmpty(value) ? null : (char?)value[0];

		public static char? ToChar(byte value) => (char?)value;
		public static char? ToChar(short value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(int value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(long value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;

		public static char? ToChar(sbyte value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(ushort value) => (char?)value;
		public static char? ToChar(uint value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(ulong value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;

		public static char? ToChar(float value) => OutOfRangeFloat(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(double value) => OutOfRangeDouble(value, char.MinValue, char.MaxValue) ? null : (char?)value;
		public static char? ToChar(decimal value) => OutOfRangeDecimal(value, char.MinValue, char.MaxValue) ? null : (char?)value;

		public static char? ToChar(byte[] value) => OutOfRangeBinary(value, sizeof(char)) ? null : (char?)BitConverter.ToChar(value, 0);

		#endregion Char

		#region DateTime

		public static DateTime? ToDateTime(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				//case bool v:
				//	return ToUInt64(v);
				//case char v:
				//	return ToUInt64(v);
				case string v:
					return ToDateTime(v);

				//case float v:
				//	return ToUInt64(v);
				//case double v:
				//	return ToUInt64(v);
				//case decimal v:
				//	return ToUInt64(v);

				//case byte v:
				//	return ToUInt64(v);
				//case short v:
				//	return ToUInt64(v);
				//case int v:
				//	return ToUInt64(v);
				case long v:
					return ToDateTime(v);

				//case sbyte v:
				//	return ToUInt64(v);
				//case ushort v:
				//	return ToUInt64(v);
				//case uint v:
				//	return ToUInt64(v);
				//case ulong v:
				//	return ToUInt64(v);

				case byte[] v:
					return ToDateTime(v);
				case DateTime v:
					return ToDateTime(v);
				//case TimeSpan v:
				//	return ToUInt64(v);
				//case XmlDocument v:
				//	return ToUInt64(v);
				//case Guid v:
				//	return ToUInt64(v);
				default:
					return TryCast<DateTime>(value);
			}
		}

		public static DateTime? ToDateTime(DateTime value) => value;
		public static DateTime? ToDateTime(string value) => DateTime.TryParse(value, out DateTime result) ? (DateTime?)result : null;

		public static DateTime? ToDateTime(long value) => value > 0 ? new DateTime(value) : null;

		public static DateTime? ToDateTime(byte[] value)
		{
			var ticks = ToInt64(value);
			return ticks == null ? null : ToDateTime(ticks.Value);
		}

		#endregion DateTime

		#region Decimal

		public static decimal? ToDecimal(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToDecimal(v);
				case char v:
					return ToDecimal(v);
				case string v:
					return ToDecimal(v);

				case float v:
					return ToDecimal(v);
				case double v:
					return ToDecimal(v);
				case decimal v:
					return ToDecimal(v);

				case byte v:
					return ToDecimal(v);
				case short v:
					return ToDecimal(v);
				case int v:
					return ToDecimal(v);
				case long v:
					return ToDecimal(v);

				case sbyte v:
					return ToDecimal(v);
				case ushort v:
					return ToDecimal(v);
				case uint v:
					return ToDecimal(v);
				case ulong v:
					return ToDecimal(v);

				case byte[] v:
					return ToDecimal(v);
				//case DateTime v:
				//	return ToDecimal(v);
				//case TimeSpan v:
				//	return ToDecimal(v);
				//case XmlDocument v:
				//	return ToDecimal(v);
				//case Guid v:
				//	return ToDecimal(v);
				default:
					return TryCast<decimal>(value);
			}
		}

		public static decimal? ToDecimal(bool value) => value ? 1 : 0;
		public static decimal? ToDecimal(char value) => value;
		public static decimal? ToDecimal(string value) => decimal.TryParse(value, out decimal result) ? (decimal?)result : null;

		public static decimal? ToDecimal(byte value) => value;
		public static decimal? ToDecimal(short value) => value;
		public static decimal? ToDecimal(int value) => value;
		public static decimal? ToDecimal(long value) => value;

		public static decimal? ToDecimal(sbyte value) => value;
		public static decimal? ToDecimal(ushort value) => value;
		public static decimal? ToDecimal(uint value) => value;
		public static decimal? ToDecimal(ulong value) => value;

		public static decimal? ToDecimal(float value) => OutOfRangeFloat(value, (float)decimal.MinValue, (float)decimal.MaxValue) ? null : (decimal?)value;
		public static decimal? ToDecimal(double value) => OutOfRangeDouble(value, (double)decimal.MinValue, (double)decimal.MaxValue) ? null : (decimal?)value;
		public static decimal? ToDecimal(decimal value) => value;

		public static decimal? ToDecimal(byte[] value)
		{
			try
			{
				if (OutOfRangeBinary(value, 16))
					return null;

				var a = BitConverter.ToInt32(value, 0);
				var b = BitConverter.ToInt32(value, 4);
				var c = BitConverter.ToInt32(value, 8);
				var d = BitConverter.ToInt32(value, 12);
				return new decimal(new int[] { a, b, c, d});
			}
			catch
			{
				return null;
			}
		}

		#endregion Decimal

		#region Double

		public static double? ToDouble(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToDouble(v);
				case char v:
					return ToDouble(v);
				case string v:
					return ToDouble(v);

				case float v:
					return ToDouble(v);
				case double v:
					return ToDouble(v);
				case decimal v:
					return ToDouble(v);

				case byte v:
					return ToDouble(v);
				case short v:
					return ToDouble(v);
				case int v:
					return ToDouble(v);
				case long v:
					return ToDouble(v);

				case sbyte v:
					return ToDouble(v);
				case ushort v:
					return ToDouble(v);
				case uint v:
					return ToDouble(v);
				case ulong v:
					return ToDouble(v);

				case byte[] v:
					return ToDouble(v);
				//case DateTime v:
				//	return ToDouble(v);
				//case TimeSpan v:
				//	return ToDouble(v);
				//case XmlDocument v:
				//	return ToDouble(v);
				//case Guid v:
				//	return ToDouble(v);
				default:
					return TryCast<double>(value);
			}
		}

		public static double? ToDouble(bool value) => (double?)(value ? 1 : 0);
		public static double? ToDouble(char value) => value;
		public static double? ToDouble(string value) => double.TryParse(value, out double result) ? (double?)result : null;

		public static double? ToDouble(byte value) => value;
		public static double? ToDouble(short value) => value;
		public static double? ToDouble(int value) => value;
		public static double? ToDouble(long value) => value;

		public static double? ToDouble(sbyte value) => value;
		public static double? ToDouble(ushort value) => value;
		public static double? ToDouble(uint value) => value;
		public static double? ToDouble(ulong value) => value;

		public static double? ToDouble(float value) => value;
		public static double? ToDouble(double value) => value;
		public static double? ToDouble(decimal value) => (double?)value;

		public static double? ToDouble(byte[] value) => OutOfRangeBinary(value, sizeof(double)) ? null : (double?)BitConverter.ToDouble(value, 0);

		#endregion Double

		#region Enum

		public static object ToEnum(object value, Type enumType)
		{
			if (value.IsNullOrDBNull())
				return null;

			if (value is string text)
				return ToEnum(text, enumType);

			Type baseType = Enum.GetUnderlyingType(enumType);
			value = ConvertTo(value, baseType);
			if (value.IsNullOrDBNull())
				return null;

			return Enum.ToObject(enumType, value);
		}

		public static object ToEnum(string value, Type enumType)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			try
			{
				return Enum.Parse(enumType, value, true);
			}
			catch
			{
				return null;
			}
		}

		#endregion Enum

		#region Guid

		public static Guid? ToGuid(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case Guid v:
					return ToGuid(v);
				case string v:
					return ToGuid(v);
				case byte[] v:
					return ToGuid(v);
				default:
					return null;
			}
		}

		public static Guid? ToGuid(Guid value) => value;
		public static Guid? ToGuid(byte[] value) => OutOfRangeBinary(value, 16) ? null : (Guid?)new Guid(value);
		public static Guid? ToGuid(string value)
		{
			try
			{
				return string.IsNullOrEmpty(value) ? (Guid?)new Guid(value) : null;
			}
			catch
			{
				return null;
			}
		}

		#endregion Guid

		#region Int16

		public static short? ToInt16(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToInt16(v);
				case char v:
					return ToInt16(v);
				case string v:
					return ToInt16(v);

				case float v:
					return ToInt16(v);
				case double v:
					return ToInt16(v);
				case decimal v:
					return ToInt16(v);

				case byte v:
					return ToInt16(v);
				case short v:
					return ToInt16(v);
				case int v:
					return ToInt16(v);
				case long v:
					return ToInt16(v);

				case sbyte v:
					return ToInt16(v);
				case ushort v:
					return ToInt16(v);
				case uint v:
					return ToInt16(v);
				case ulong v:
					return ToInt16(v);

				case byte[] v:
					return ToInt16(v);
				//case DateTime v:
				//	return ToInt16(v);
				//case TimeSpan v:
				//	return ToInt16(v);
				//case XmlDocument v:
				//	return ToInt16(v);
				//case Guid v:
				//	return ToInt16(v);
				default:
					return TryCast<short>(value);
			}
		}

		public static short? ToInt16(bool value) => (short?)(value ? 1 : 0);
		public static short? ToInt16(char value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(string value) => short.TryParse(value, out short result) ? (short?)result : null;

		public static short? ToInt16(byte value) => value;
		public static short? ToInt16(short value) => value;
		public static short? ToInt16(int value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(long value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;

		public static short? ToInt16(sbyte value) => value;
		public static short? ToInt16(ushort value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(uint value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(ulong value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;

		public static short? ToInt16(float value) => OutOfRangeFloat(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(double value) => OutOfRangeDouble(value, short.MinValue, short.MaxValue) ? null : (short?)value;
		public static short? ToInt16(decimal value) => OutOfRangeDecimal(value, short.MinValue, short.MaxValue) ? null : (short?)value;

		public static short? ToInt16(byte[] value) => OutOfRangeBinary(value, sizeof(short)) ? null : (short?)BitConverter.ToInt16(value, 0);

		#endregion Int16

		#region Int32

		public static int? ToInt32(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToInt32(v);
				case char v:
					return ToInt32(v);
				case string v:
					return ToInt32(v);

				case float v:
					return ToInt32(v);
				case double v:
					return ToInt32(v);
				case decimal v:
					return ToInt32(v);

				case byte v:
					return ToInt32(v);
				case short v:
					return ToInt32(v);
				case int v:
					return ToInt32(v);
				case long v:
					return ToInt32(v);

				case sbyte v:
					return ToInt32(v);
				case ushort v:
					return ToInt32(v);
				case uint v:
					return ToInt32(v);
				case ulong v:
					return ToInt32(v);

				case byte[] v:
					return ToInt32(v);
				//case DateTime v:
				//	return ToInt32(v);
				//case TimeSpan v:
				//	return ToInt32(v);
				//case XmlDocument v:
				//	return ToInt32(v);
				//case Guid v:
				//	return ToInt32(v);
				default:
					return TryCast<int>(value);
			}
		}

		public static int? ToInt32(bool value) => (int?)(value ? 1 : 0);
		public static int? ToInt32(char value) => value;
		public static int? ToInt32(string value) => int.TryParse(value, out int result) ? (int?)result : null;

		public static int? ToInt32(byte value) => value;
		public static int? ToInt32(short value) => value;
		public static int? ToInt32(int value) => value;
		public static int? ToInt32(long value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (int?)value;

		public static int? ToInt32(sbyte value) => value;
		public static int? ToInt32(ushort value) => value;
		public static int? ToInt32(uint value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (int?)value;
		public static int? ToInt32(ulong value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (int?)value;

		public static int? ToInt32(float value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (int?)value;
		public static int? ToInt32(double value) => OutOfRangeDouble(value, int.MinValue, int.MaxValue) ? null : (int?)value;
		public static int? ToInt32(decimal value) => OutOfRangeDecimal(value, int.MinValue, int.MaxValue) ? null : (int?)value;

		public static int? ToInt32(byte[] value) => OutOfRangeBinary(value, sizeof(int)) ? null : (int?)BitConverter.ToInt32(value, 0);

		#endregion Int32

		#region Int64

		public static long? ToInt64(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToInt64(v);
				case char v:
					return ToInt64(v);
				case string v:
					return ToInt64(v);

				case float v:
					return ToInt64(v);
				case double v:
					return ToInt64(v);
				case decimal v:
					return ToInt64(v);

				case byte v:
					return ToInt64(v);
				case short v:
					return ToInt64(v);
				case int v:
					return ToInt64(v);
				case long v:
					return ToInt64(v);

				case sbyte v:
					return ToInt64(v);
				case ushort v:
					return ToInt64(v);
				case uint v:
					return ToInt64(v);
				case ulong v:
					return ToInt64(v);

				case byte[] v:
					return ToInt64(v);
				//case DateTime v:
				//	return ToInt64(v);
				//case TimeSpan v:
				//	return ToInt64(v);
				//case XmlDocument v:
				//	return ToInt64(v);
				//case Guid v:
				//	return ToInt64(v);
				default:
					return TryCast<long>(value);
			}
		}

		public static long? ToInt64(bool value) => (long?)(value ? 1 : 0);
		public static long? ToInt64(char value) => value;
		public static long? ToInt64(string value) => long.TryParse(value, out long result) ? (long?)result : null;

		public static long? ToInt64(byte value) => value;
		public static long? ToInt64(short value) => value;
		public static long? ToInt64(int value) => value;
		public static long? ToInt64(long value) => value;

		public static long? ToInt64(sbyte value) => value;
		public static long? ToInt64(ushort value) => value;
		public static long? ToInt64(uint value) => value;
		public static long? ToInt64(ulong value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (long?)value;

		public static long? ToInt64(float value) => OutOfRangeFloat(value, int.MinValue, int.MaxValue) ? null : (long?)value;
		public static long? ToInt64(double value) => OutOfRangeDouble(value, int.MinValue, int.MaxValue) ? null : (long?)value;
		public static long? ToInt64(decimal value) => OutOfRangeDecimal(value, long.MinValue, long.MaxValue) ? null : (long?)value;

		public static long? ToInt64(byte[] value) => OutOfRangeBinary(value, sizeof(long)) ? null : (long?)BitConverter.ToInt64(value, 0);
		public static long? ToInt64(DateTime value) => value.Ticks;

		#endregion Int64

		#region SByte

		public static sbyte? ToSByte(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToSByte(v);
				case char v:
					return ToSByte(v);
				case string v:
					return ToSByte(v);

				case float v:
					return ToSByte(v);
				case double v:
					return ToSByte(v);
				case decimal v:
					return ToSByte(v);

				case byte v:
					return ToSByte(v);
				case short v:
					return ToSByte(v);
				case int v:
					return ToSByte(v);
				case long v:
					return ToSByte(v);

				case sbyte v:
					return ToSByte(v);
				case ushort v:
					return ToSByte(v);
				case uint v:
					return ToSByte(v);
				case ulong v:
					return ToSByte(v);

				case byte[] v:
					return ToSByte(v);
				//case DateTime v:
				//	return ToSByte(v);
				//case TimeSpan v:
				//	return ToSByte(v);
				//case XmlDocument v:
				//	return ToSByte(v);
				//case Guid v:
				//	return ToSByte(v);
				default:
					return TryCast<sbyte>(value);
			}
		}

		public static sbyte? ToSByte(bool value) => (sbyte?)(value ? 1 : 0);
		public static sbyte? ToSByte(char value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(string value) => sbyte.TryParse(value, out sbyte result) ? (sbyte?)result : null;

		public static sbyte? ToSByte(byte value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(short value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(int value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(long value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;

		public static sbyte? ToSByte(sbyte value) => value;
		public static sbyte? ToSByte(ushort value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(uint value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(ulong value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;

		public static sbyte? ToSByte(float value) => OutOfRangeFloat(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(double value) => OutOfRangeDouble(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;
		public static sbyte? ToSByte(decimal value) => OutOfRangeDecimal(value, sbyte.MinValue, sbyte.MaxValue) ? null : (sbyte?)value;

		public static sbyte? ToSByte(byte[] value) => OutOfRangeBinary(value, sizeof(sbyte)) ? null : (sbyte?)value[0];

		#endregion SByte

		#region Single

		public static float? ToSingle(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToSingle(v);
				case char v:
					return ToSingle(v);
				case string v:
					return ToSingle(v);

				case float v:
					return ToSingle(v);
				case double v:
					return ToSingle(v);
				case decimal v:
					return ToSingle(v);

				case byte v:
					return ToSingle(v);
				case short v:
					return ToSingle(v);
				case int v:
					return ToSingle(v);
				case long v:
					return ToSingle(v);

				case sbyte v:
					return ToSingle(v);
				case ushort v:
					return ToSingle(v);
				case uint v:
					return ToSingle(v);
				case ulong v:
					return ToSingle(v);

				case byte[] v:
					return ToSingle(v);
				//case DateTime v:
				//	return ToSingle(v);
				//case TimeSpan v:
				//	return ToSingle(v);
				//case XmlDocument v:
				//	return ToSingle(v);
				//case Guid v:
				//	return ToSingle(v);
				default:
					return TryCast<float>(value);
			}
		}

		public static float? ToSingle(bool value) => (float?)(value ? 1 : 0);
		public static float? ToSingle(char value) => value;
		public static float? ToSingle(string value) => float.TryParse(value, out float result) ? (float?)result : null;

		public static float? ToSingle(byte value) => value;
		public static float? ToSingle(short value) => value;
		public static float? ToSingle(int value) => value;
		public static float? ToSingle(long value) => value;

		public static float? ToSingle(sbyte value) => value;
		public static float? ToSingle(ushort value) => value;
		public static float? ToSingle(uint value) => value;
		public static float? ToSingle(ulong value) => value;

		public static float? ToSingle(float value) => value;
		public static float? ToSingle(double value) => OutOfRangeDouble(value, float.MinValue, float.MaxValue) ? null : (float?)value;
		public static float? ToSingle(decimal value) => (float?)value;

		public static float? ToSingle(byte[] value) => OutOfRangeBinary(value, sizeof(float)) ? null : (float?)BitConverter.ToSingle(value, 0);

		#endregion Single

		#region String

		public static string ToString(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToString(v);
				case char v:
					return ToString(v);
				case string v:
					return ToString(v);

				case float v:
					return ToString(v);
				case double v:
					return ToString(v);
				case decimal v:
					return ToString(v);

				case byte v:
					return ToString(v);
				case short v:
					return ToString(v);
				case int v:
					return ToString(v);
				case long v:
					return ToString(v);

				case sbyte v:
					return ToString(v);
				case ushort v:
					return ToString(v);
				case uint v:
					return ToString(v);
				case ulong v:
					return ToString(v);

				case byte[] v:
					return ToString(v);
				case DateTime v:
					return ToString(v);
				//case TimeSpan v:
				//	return ToString(v);
				//case XmlDocument v:
				//	return ToString(v);
				//case Guid v:
				//	return ToString(v);
				default:
					return value.ToString();
			}
		}

		public static string ToString(bool value) => value ? bool.TrueString : bool.FalseString;
		public static string ToString(char value) => new string(value, 1);
		public static string ToString(string value) => value;

		public static string ToString(byte value) => value.ToString();
		public static string ToString(short value) => value.ToString();
		public static string ToString(int value) => value.ToString();
		public static string ToString(long value) => value.ToString();

		public static string ToString(sbyte value) => value.ToString();
		public static string ToString(ushort value) => value.ToString();
		public static string ToString(uint value) => value.ToString();
		public static string ToString(ulong value) => value.ToString();

		public static string ToString(float value) => value.ToString();
		public static string ToString(double value) => value.ToString();
		public static string ToString(decimal value) => value.ToString();

		public static string ToString(byte[] value) => OutOfRangeBinary(value, 0) ? null : Encoding.UTF8.GetString(value);
		public static string ToString(DateTime value) => value.ToString();

		#endregion String

		#region UInt16

		public static ushort? ToUInt16(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToUInt16(v);
				case char v:
					return ToUInt16(v);
				case string v:
					return ToUInt16(v);

				case float v:
					return ToUInt16(v);
				case double v:
					return ToUInt16(v);
				case decimal v:
					return ToUInt16(v);

				case byte v:
					return ToUInt16(v);
				case short v:
					return ToUInt16(v);
				case int v:
					return ToUInt16(v);
				case long v:
					return ToUInt16(v);

				case sbyte v:
					return ToUInt16(v);
				case ushort v:
					return ToUInt16(v);
				case uint v:
					return ToUInt16(v);
				case ulong v:
					return ToUInt16(v);

				case byte[] v:
					return ToUInt16(v);
				//case DateTime v:
				//	return ToUInt16(v);
				//case TimeSpan v:
				//	return ToUInt16(v);
				//case XmlDocument v:
				//	return ToUInt16(v);
				//case Guid v:
				//	return ToUInt16(v);
				default:
					return TryCast<ushort>(value);
			}
		}

		public static ushort? ToUInt16(bool value) => (ushort?)(value ? 1 : 0);
		public static ushort? ToUInt16(char value) => value;
		public static ushort? ToUInt16(string value) => ushort.TryParse(value, out ushort result) ? (ushort?)result : null;

		public static ushort? ToUInt16(byte value) => value;
		public static ushort? ToUInt16(short value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(int value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(long value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;

		public static ushort? ToUInt16(sbyte value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(ushort value) => value;
		public static ushort? ToUInt16(uint value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(ulong value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;

		public static ushort? ToUInt16(float value) => OutOfRangeFloat(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(double value) => OutOfRangeDouble(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;
		public static ushort? ToUInt16(decimal value) => OutOfRangeDecimal(value, ushort.MinValue, ushort.MaxValue) ? null : (ushort?)value;

		public static ushort? ToUInt16(byte[] value) => OutOfRangeBinary(value, sizeof(ushort)) ? null : (ushort?)BitConverter.ToUInt16(value, 0);

		#endregion UInt16

		#region UInt32

		public static uint? ToUInt32(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToUInt32(v);
				case char v:
					return ToUInt32(v);
				case string v:
					return ToUInt32(v);

				case float v:
					return ToUInt32(v);
				case double v:
					return ToUInt32(v);
				case decimal v:
					return ToUInt32(v);

				case byte v:
					return ToUInt32(v);
				case short v:
					return ToUInt32(v);
				case int v:
					return ToUInt32(v);
				case long v:
					return ToUInt32(v);

				case sbyte v:
					return ToUInt32(v);
				case ushort v:
					return ToUInt32(v);
				case uint v:
					return ToUInt32(v);
				case ulong v:
					return ToUInt32(v);

				case byte[] v:
					return ToUInt32(v);
				//case DateTime v:
				//	return ToUInt32(v);
				//case TimeSpan v:
				//	return ToUInt32(v);
				//case XmlDocument v:
				//	return ToUInt32(v);
				//case Guid v:
				//	return ToUInt32(v);
				default:
					return TryCast<uint>(value);
			}
		}

		public static uint? ToUInt32(bool value) => (uint?)(value ? 1 : 0);
		public static uint? ToUInt32(char value) => value;
		public static uint? ToUInt32(string value) => uint.TryParse(value, out uint result) ? (uint?)result : null;

		public static uint? ToUInt32(byte value) => value;
		public static uint? ToUInt32(short value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;
		public static uint? ToUInt32(int value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;
		public static uint? ToUInt32(long value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;

		public static uint? ToUInt32(sbyte value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;
		public static uint? ToUInt32(ushort value) => value;
		public static uint? ToUInt32(uint value) => value;
		public static uint? ToUInt32(ulong value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;

		public static uint? ToUInt32(float value) => OutOfRangeFloat(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;
		public static uint? ToUInt32(double value) => OutOfRangeDouble(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;
		public static uint? ToUInt32(decimal value) => OutOfRangeDecimal(value, uint.MinValue, uint.MaxValue) ? null : (uint?)value;

		public static uint? ToUInt32(byte[] value) => OutOfRangeBinary(value, sizeof(uint)) ? null : (uint?)BitConverter.ToUInt32(value, 0);

		#endregion UInt32

		#region UInt64

		public static ulong? ToUInt64(object value)
		{
			if (value.IsNullOrDBNull())
				return null;

			switch (value)
			{
				case bool v:
					return ToUInt64(v);
				case char v:
					return ToUInt64(v);
				case string v:
					return ToUInt64(v);

				case float v:
					return ToUInt64(v);
				case double v:
					return ToUInt64(v);
				case decimal v:
					return ToUInt64(v);

				case byte v:
					return ToUInt64(v);
				case short v:
					return ToUInt64(v);
				case int v:
					return ToUInt64(v);
				case long v:
					return ToUInt64(v);

				case sbyte v:
					return ToUInt64(v);
				case ushort v:
					return ToUInt64(v);
				case uint v:
					return ToUInt64(v);
				case ulong v:
					return ToUInt64(v);

				case byte[] v:
					return ToUInt64(v);
				//case DateTime v:
				//	return ToUInt64(v);
				//case TimeSpan v:
				//	return ToUInt64(v);
				//case XmlDocument v:
				//	return ToUInt64(v);
				//case Guid v:
				//	return ToUInt64(v);
				default:
					return TryCast<ulong>(value);
			}
		}

		public static ulong? ToUInt64(bool value) => (ulong?)(value ? 1 : 0);
		public static ulong? ToUInt64(char value) => value;
		public static ulong? ToUInt64(string value) => ulong.TryParse(value, out ulong result) ? (ulong?)result : null;

		public static ulong? ToUInt64(byte value) => value;
		public static ulong? ToUInt64(short value) => OutOfRangeFloat(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;
		public static ulong? ToUInt64(int value) => OutOfRangeFloat(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;
		public static ulong? ToUInt64(long value) => OutOfRangeFloat(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;

		public static ulong? ToUInt64(sbyte value) => OutOfRangeFloat(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;
		public static ulong? ToUInt64(ushort value) => value;
		public static ulong? ToUInt64(uint value) => value;
		public static ulong? ToUInt64(ulong value) => value;

		public static ulong? ToUInt64(float value) => OutOfRangeFloat(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;
		public static ulong? ToUInt64(double value) => OutOfRangeDouble(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;
		public static ulong? ToUInt64(decimal value) => OutOfRangeDecimal(value, ulong.MinValue, ulong.MaxValue) ? null : (ulong?)value;

		public static ulong? ToUInt64(byte[] value) => OutOfRangeBinary(value, sizeof(ulong)) ? null : (ulong?)BitConverter.ToUInt64(value, 0);

		#endregion UInt64
	}
}
