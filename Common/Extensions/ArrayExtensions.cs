using System;

namespace TheLion.Common
{
	public static class ArrayExtensions
	{
		/// <summary>Get a subset of the calling array.</summary>
		/// <param name="index">The starting index.</param>
		/// <param name="length">The subset length.</param>
		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			var result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}
	}
}