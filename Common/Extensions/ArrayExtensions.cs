using System;

namespace TheLion.Stardew.Common.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>Get a subarray from the calling array.</summary>
		/// <param name="index">The starting index.</param>
		/// <param name="length">The subarray length.</param>
		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			var result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}
	}
}