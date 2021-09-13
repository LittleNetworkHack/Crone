using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public sealed class ObjectComparer<T> : IEqualityComparer<T>, IEqualityComparer, IComparer<T>, IComparer
	{
		private static bool ValueType = typeof(T).IsValueType;
		private static PropertyKeyCollection properties = PropertyKey.GetKeys<T>();

		public static ObjectComparer<T> Comparer { get; } = new ObjectComparer<T>();

		#region IEqualityComparer<T>

		public bool Equals(T x, T y)
		{
			if (ValueType)
				return EqualityComparer<T>.Default.Equals(x, y);

			if (ReferenceEquals(x, y))
				return true;

			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;

			foreach (IEqualityComparer<T> key in properties)
			{
				if (key.Equals(x, y))
					continue;

				return false;
			}

			return true;
		}

		public int GetHashCode(T obj)
		{
			return obj?.GetHashCode() ?? 0;
		}

		#endregion IEqualityComparer<T>

		#region IEqualityComparer

		bool IEqualityComparer.Equals(object x, object y)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x is T cx && y is T cy)
				return Equals(cx, cy);

			return false;
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			return obj?.GetHashCode() ?? 0;
		}

		#endregion IEqualityComparer

		#region IComparer

		int IComparer<T>.Compare(T x, T y)
		{
			return 0;
		}

		int IComparer.Compare(object x, object y)
		{
			return 0;
		}

		#endregion IComparer
	}
}
