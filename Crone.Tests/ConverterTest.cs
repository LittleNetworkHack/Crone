using System;
using Xunit;

namespace Crone.Tests
{
	public class ConverterTest
	{
		[Fact]
		public void Bool_To_Int32()
		{
			var value = true;
			var expected = 1;
			var actual = ValueConverter.ConvertTo<int?>(value);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Int32_To_Bool()
		{
			var value = 5;
			var expected = true;
			var actual = ValueConverter.ConvertTo<bool?>(value);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void String_To_Int32()
		{
			var value = "aaa";
			var expected = default(int?);
			var actual = ValueConverter.ConvertTo<int?>(value);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void String_To_DateTime()
		{
			var value = "01/01/2020";
			var expected = new DateTime(2020, 1, 1);
			var actual = ValueConverter.ConvertTo<DateTime?>(value);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ByteArray_To_Decimal()
		{
			var value = new byte[] { 64, 226, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0 };
			var expected = 123.456M;
			var actual = ValueConverter.ConvertTo<decimal?>(value);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Decimal_To_ByteArray()
		{
			var value = 123.456M;
			var expected = new byte[] { 64, 226, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0 };
			var actual = ValueConverter.ConvertTo<byte[]>(value);
			Assert.Equal(expected, actual);
		}
	}
}
