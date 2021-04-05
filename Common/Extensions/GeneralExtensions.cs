using System.Linq;

namespace TheLion.Common
{
	public static class GeneralExtensions
	{
		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="collection">A sequence of objects.</param>
		public static bool AnyOf<T>(this T source, params T[] collection)
		{
			return collection.Contains(source);
		}
	}
}