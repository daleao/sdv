using System.Linq;

namespace TheLion.Common.Extensions
{
	public static class Extensions
	{
		/// <summary>Determines if an instance is contained in a sequence.</summary>
		public static bool IsIn<T>(this T obj, params T[] collection)
		{
			return collection.Contains(obj);
		}
	}
}
