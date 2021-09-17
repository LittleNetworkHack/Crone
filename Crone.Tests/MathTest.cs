using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Crone.Tests
{
	public class MathTest
	{
		[Fact]
		public void ValidIndexInRange()
		{
			var list = Enumerable.Range(1, 10).ToArray();
			Assert.True(list.Length.IndexInRange(2));
		}

		[Fact]
		public void InvalidIndexInRange()
		{
			var list = Enumerable.Range(1, 10).ToArray();
			Assert.False(list.Length.IndexInRange(20));
		}

		[Fact]
		public void ProperDivision()
		{
			var value = 26M;
			var count = 6;
			var round = 2;
			var expected = new decimal[] { 4.33M, 4.33M, 4.34M, 4.33M, 4.34M, 4.33M };
			var actual = value.DivideEvenly(count, round).ToArray();
			Assert.Equal(expected, actual);
		}
	}
}
