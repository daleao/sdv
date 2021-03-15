using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Common.Extensions
{
	public static class Extensions
	{
		/// <summary>Determine if an instance is contained in a sequence.</summary>
		/// <param name="item">The instance to search for.</param>
		/// <param name="collection">The collection to be searched.</param>
		public static bool AnyOf<T>(this T item, params T[] collection)
		{
			return collection.Contains(item);
		}

		/// <summary>Get the last item in a list.</summary>
		/// <param name="list">The list to be searched.</param>
		public static T Last<T>(this IList<T> list)
		{
			if (list.Count() > 0)
			{ 
				return list[list.Count() - 1];
			}

			return default;
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

		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2(p.X, p.Y);
		}
	}
}
