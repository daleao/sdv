using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace TheLion.Common.Harmony
{
	public static class CodeInstructionListExtensions
	{
		/// <summary>Determine the index of an instruction pattern in a list of code instructions.</summary>
		/// <param name="list">The list to be searched.</param>
		/// <param name="pattern">The pattern to search for.</param>
		/// <param name="start">The starting index.</param>
		public static int IndexOf(this IList<CodeInstruction> list, CodeInstruction[] pattern, int start = 0)
		{
			int count = list.Count() - pattern.Count() + 1;
			for (int i = start; i < count; ++i)
			{
				int j = 0;
				while (j < pattern.Count() && list[i + j].opcode.Equals(pattern[j].opcode)
					&& (pattern[j].operand == null || list[i + j].operand.ToString().Equals(pattern[j].operand.ToString())))
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

		/// <summary>Determine the index of the code instruction with a certain branch label in a list of code instructions.</summary>
		/// <param name="list">The list to be searched.</param>
		/// <param name="pattern">The pattern to search for.</param>
		/// <param name="start">The starting index.</param>
		public static int IndexOf(this IList<CodeInstruction> list, Label label, int start = 0)
		{
			int count = list.Count();
			for (int i = start; i < count; ++i)
			{
				if (list[i].labels.Contains(label))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>Deep copy a list of code instructions.</summary>
		/// <param name="list">The list to be copied.</param>
		public static List<CodeInstruction> Clone(this IList<CodeInstruction> list)
		{
			return list.Select(instruction => new CodeInstruction(instruction)).ToList();
		}

		/// <summary>Deep copy a list of labels.</summary>
		/// <param name="list">The list to be copied.</param>
		public static List<Label> Clone(this IList<Label> list)
		{
			List<Label> clone = new();
			foreach (Label label in list) clone.Add(label);
			return clone;
		}
	}
}
