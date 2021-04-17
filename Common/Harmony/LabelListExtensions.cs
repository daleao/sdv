using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.Common.Harmony
{
	public static class LabelListExtensions
	{
		/// <summary>Deep copy a list of labels.</summary>
		/// <param name="list">The list to be copied.</param>
		public static List<Label> Clone(this IList<Label> list)
		{
			List<Label> clone = new();
			foreach (var label in list) clone.Add(label);
			return clone;
		}
	}
}