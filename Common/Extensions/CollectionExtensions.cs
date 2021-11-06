using System;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Stardew.Common.Extensions
{
	public static class CollectionExtensions
	{
		/// <summary>Determine if a collection contains any of the objects in a sequence.</summary>
		/// <param name="candidates">The objects to search for.</param>
		public static bool ContainsAnyOf<T>(this ICollection<T> collection, params T[] candidates)
		{
			return candidates.Any(collection.Contains);
		}

		/// <summary>Determine if a collection contains any instance of the given types.</summary>
		/// <param name="types">The types to search for.</param>
		public static bool ContainsType<T>(this ICollection<T> collection, Type type)
		{
			return collection.Any(item => item is not null && item.GetType() == type);
		}

		/// <summary>Determine if a collection contains any instance of the given types.</summary>
		/// <param name="types">The types to search for.</param>
		public static bool ContainsAnyOfTypes<T>(this ICollection<T> collection, params Type[] types)
		{
			return collection.Any(item => item is not null && item.IsAnyOfTypes(types));
		}

		/// <summary>Remove the first instance of a given type from a collection.</summary>
		/// <param name="type">The type to search for.</param>
		/// <param name="removed">The removed instance.</param>
		/// <returns>Returns true if an instance was successfully removed, else returns false.</returns>
		public static bool RemoveType<T>(this ICollection<T> collection, Type type, out T removed)
		{
			var toRemove = collection.SingleOrDefault(item => item is not null && item.GetType() == type);
			if (toRemove is not null)
			{
				removed = toRemove;
				return collection.Remove(toRemove);
			}

			removed = default;
			return false;
		}
	}
}