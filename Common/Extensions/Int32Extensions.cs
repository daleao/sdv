using System.Linq;

namespace TheLion.Common
{
	public static class Int32Extensions
	{
		/// <summary>Raise a number to an integer power.</summary>
		/// <param name="exp">Positive integer exponent.</param>
		public static int Pow(this int num, int exp)
		{
			return Enumerable
				.Repeat(num, exp)
				.Aggregate(1, (a, b) => a * b);
		}
	}
}