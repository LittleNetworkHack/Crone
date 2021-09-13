using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public struct Currency
	{
		private readonly long Value;

		public Currency(long value) => Value = value;
		public Currency(decimal value) => Value = decimal.ToOACurrency(value);

		public static implicit operator Currency(long value) => new Currency(value);
		public static implicit operator long(Currency value) => value.Value;

		public static implicit operator Currency(decimal value) => new Currency(value);
		public static implicit operator decimal(Currency value) => decimal.FromOACurrency(value.Value);

		public override string ToString()
			=> ((decimal)this).ToString("0.00##");
	}
}
