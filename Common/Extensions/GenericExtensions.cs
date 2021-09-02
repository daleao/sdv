using System;
using System.Linq;

namespace TheLion.Stardew.Common.Extensions
{
	public static class GeneralExtensions
	{
		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="collection">A sequence of objects.</param>
		public static bool AnyOf<T>(this T obj, params T[] collection)
		{
			return collection.Contains(obj);
		}

		/// <summary>Determine if the calling object's type is equivalent to any of the types in a sequence.</summary>
		/// <param name="types">A sequence of <see cref="Type"/>'s.</param>
		public static bool AnyOfType<T>(this T t, params Type[] types)
		{
			return t.GetType().AnyOf(types);
		}

		/// <summary>Convert the calling object to a generic type.</summary>
		public static T ChangeType<T>(this object obj) where T : IConvertible
		{
			return (T)Convert.ChangeType(obj, typeof(T));
		}
	}
}