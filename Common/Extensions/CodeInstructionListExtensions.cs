using Harmony;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Common.Harmony
{
	public static class CodeInstructionListExtensions
	{
		/// <summary>Determine the index of a code instruction in a list of instructions.</summary>
		/// <param name="list">The list to be searched.</param>
		/// <param name="pattern">The pattern to search for.</param>
		/// <param name="start">The starting index.</param>
		public static int IndexOf(this IList<CodeInstruction> list, CodeInstruction[] pattern, int start = 0)
		{
			for (int i = start; i < list.Count() - pattern.Count() + 1; ++i)
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

		public static List<CodeInstruction> Clone(this IList<CodeInstruction> list)
		{
			return list.Select(instruction => new CodeInstruction(instruction)).ToList();
		}
	}
}
