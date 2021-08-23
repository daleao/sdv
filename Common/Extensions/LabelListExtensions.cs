using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.Stardew.Common.Extensions
{
	public static class LabelListExtensions
	{
		/// <summary>Deep copy a list of labels.</summary>
		public static List<Label> Clone(this IList<Label> list)
		{
			List<Label> clone = new();
			foreach (var label in list) clone.Add(label);
			return clone;
		}
	}
}