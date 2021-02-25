using Harmony;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Common.Extensions
{
	public static class Extensions
	{
		/// <summary>Determine if an instance is contained in a sequence.</summary>
		/// <param name="obj">The instance to search for.</param>
		/// <param name="collection">The collection to be searched.</param>
		public static bool IsIn<T>(this T obj, params T[] collection)
		{
			return collection.Contains(obj);
		}

		/// <summary>Determine the index of an item in a list.</summary>
		/// <param name="list">The list to be searched.</param>
		/// <param name="pattern">The pattern to search for.</param>
		/// <param name="start">The starting index.</param>
		public static int IndexOf<T>(this IList<T> list, T[] pattern, int start = 0)
		{
			for (int i = start; i < list.Count() - pattern.Count() + 1; ++i)
			{
				int j = 0;
				while (j < pattern.Count() && list[i + j].Equals(pattern[j]))
				{
					++j;
				}
				if (j == pattern.Count())
				{
					return i;
				}
			}

			return -1;
		}
	}
}
