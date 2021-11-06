using System;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Stardew.Common.Extensions
{
	public static class GeneralExtensions
	{
		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="candidates">A sequence of <typeparamref name="T" /> objects.</param>
		public static bool IsAnyOf<T>(this T obj, params T[] candidates)
		{
			return candidates.Contains(obj);
		}

		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="candidates">A sequence of <typeparamref name="T" /> objects.</param>
		public static bool IsAnyOf<T>(this T obj, IEnumerable<T> candidates)
		{
			return candidates.Contains(obj);
		}

		/// <summary>Determine if the calling object's type is equivalent to any of the types in a sequence.</summary>
		/// <param name="types">A sequence of <see cref="Type" />'s.</param>
		public static bool IsAnyOfTypes<T>(this T t, params Type[] types)
		{
			return t.GetType().IsAnyOf(types);
		}
	}
}