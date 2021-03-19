namespace TheLion.Common.Classes
{
	public static class RationalMath
	{
		/// <summary>Clamp an integer between inclusive lower and upper bounds.</summary>
		/// <param name="val">The value to clamp.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		public static int Clamp(int val, int min, int max)
		{
			if (val < min) return min;
			if (val > max) return max;
			return val;
		}
	}
}
