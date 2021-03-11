using System;

namespace TheLion.Common
{
	public static class Utils
	{
		/// <summary>Get the first n digits of the hash code for obj.</summary>
		/// <param name="obj">The object.</param>
		/// <param name="n">The number of digits.</param>
		public static int GetDigitsFromHash(object obj, int n)
		{
			if (obj == null)
				throw new ArgumentNullException("Object cannot be null.");

			if (n < 1)
				throw new ArgumentException("Number of digits must be positive.");

			return (int)(Math.Abs(obj.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(obj.GetHashCode()))) - n + 1));
		}
	}
}
