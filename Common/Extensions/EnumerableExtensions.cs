using System;
using System.Collections.Generic;

namespace TheLion.Stardew.Common.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>Apply an action to each item in <see cref="IEnumerable{T}" />.</summary>
		/// <param name="action">An action to apply.</param>
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items) action(item);
		}
	}
}