using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace DaLion.Stardew.Common.Extensions;

public static class CodeInstructionListExtensions
{
    /// <summary>Determine the index of the first occurrence of an instruction pattern.</summary>
    /// <param name="pattern">The <see cref="CodeInstruction" /> pattern to search for.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>Returns the index of the first instruction in <paramref name="pattern" />.</returns>
    public static int IndexOf(this IList<CodeInstruction> list, CodeInstruction[] pattern, int start = 0)
    {
        var count = list.Count - pattern.Length + 1;
        for (var i = start; i < count; ++i)
        {
            var j = 0;
            while (j < pattern.Length && list[i + j].opcode.Equals(pattern[j].opcode)
                                      && (pattern[j].operand is null || list[i + j].operand?.ToString()
                                          == pattern[j].operand.ToString()))
                ++j;
            if (j == pattern.Length) return i;
        }

        return -1;
    }

    /// <summary>Determine the index of the first occurrence of an instruction pattern.</summary>
    /// <param name="pattern">The <see cref="CodeInstruction" /> pattern to search for.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>Returns the index of the first instruction in <paramref name="pattern" />.</returns>
    public static int IndexOf(this IList<CodeInstruction> list, IList<CodeInstruction> pattern, int start = 0)
    {
        var count = list.Count - pattern.Count + 1;
        for (var i = start; i < count; ++i)
        {
            var j = 0;
            while (j < pattern.Count && list[i + j].opcode.Equals(pattern[j].opcode)
                                     && (pattern[j].operand is null || list[i + j].operand?.ToString()
                                         == pattern[j].operand.ToString()))
                ++j;
            if (j == pattern.Count) return i;
        }

        return -1;
    }

    /// <summary>Determine the index of the first code instruction that contains a certain branch label.</summary>
    /// <param name="label">The <see cref="Label" /> object to search for.</param>
    /// <param name="start">The starting index.</param>
    public static int IndexOf(this IList<CodeInstruction> list, Label label, int start = 0)
    {
        var count = list.Count;
        for (var i = start; i < count; ++i)
            if (list[i].labels.Contains(label))
                return i;

        return -1;
    }

    /// <summary>Deep copy a list of code instructions.</summary>
    public static List<CodeInstruction> Clone(this IList<CodeInstruction> list)
    {
        return list.Select(instruction => new CodeInstruction(instruction)).ToList();
    }
}