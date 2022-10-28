namespace DaLion.Shared.Harmony;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for <see cref="List{T}"/> of <see cref="CodeInstruction"/>s.</summary>
public static class CodeInstructionListExtensions
{
    /// <summary>
    ///     Finds the index of the next occurrence of the <paramref name="pattern"/> in the
    ///     <paramref name="instructions"/>s, beginning at <paramref name="start"/>.
    /// </summary>
    /// <param name="instructions">A <see cref="List{T}"/> of <see cref="CodeInstruction"/>s.</param>
    /// <param name="pattern">The <see cref="CodeInstruction"/> pattern to search for.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>The index of the first instruction in the next occurrence of <paramref name="pattern"/>.</returns>
    public static int IndexOf(this List<CodeInstruction> instructions, CodeInstruction[] pattern, int start = 0)
    {
        var count = instructions.Count - pattern.Length + 1;
        for (var i = start; i < count; ++i)
        {
            var j = 0;
            while (j < pattern.Length && instructions[i + j].opcode.Equals(pattern[j].opcode)
                                      && (pattern[j].operand is null || instructions[i + j].operand?.ToString()
                                          == pattern[j].operand.ToString()))
            {
                ++j;
            }

            if (j == pattern.Length)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Finds the index of the next occurrence of the <paramref name="pattern"/> in the
    ///     <paramref name="instructions"/>s, beginning at <paramref name="start"/>.
    /// </summary>
    /// <param name="instructions">A <see cref="List{T}"/> of <see cref="CodeInstruction"/>s.</param>
    /// <param name="pattern">The <see cref="CodeInstruction"/> pattern to search for.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>The index of the first instruction in the next occurrence of <paramref name="pattern"/>.</returns>
    public static int IndexOf(this List<CodeInstruction> instructions, IList<CodeInstruction> pattern, int start = 0)
    {
        var count = instructions.Count - pattern.Count + 1;
        for (var i = start; i < count; ++i)
        {
            var j = 0;
            while (j < pattern.Count && instructions[i + j].opcode.Equals(pattern[j].opcode)
                                     && (pattern[j].operand is null || instructions[i + j].operand?.ToString()
                                         == pattern[j].operand.ToString()))
            {
                ++j;
            }

            if (j == pattern.Count)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Finds the index of the next <see cref="CodeInstruction"/> containing the <paramref name="label"/> in the
    ///     <paramref name="instructions"/>s, beginning at <paramref name="start"/>.
    /// </summary>
    /// <param name="instructions">A <see cref="List{T}"/> of <see cref="CodeInstruction"/>s.</param>
    /// <param name="label">The <see cref="Label"/> object to search for.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>The index of the next occurrence of <paramref name="label"/> in <paramref name="instructions"/>.</returns>
    public static int IndexOf(this List<CodeInstruction> instructions, Label label, int start = 0)
    {
        var count = instructions.Count;
        for (var i = start; i < count; ++i)
        {
            if (instructions[i].labels.Contains(label))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>Creates a deep copy of the <paramref name="instructions"/>.</summary>
    /// <param name="instructions">A <see cref="List{T}"/> of <see cref="CodeInstruction"/>s.</param>
    /// <returns>An exact copy of <paramref name="instructions"/> in a new <see cref="List{T}"/> instance.</returns>
    public static List<CodeInstruction> Clone(this List<CodeInstruction> instructions)
    {
        return instructions.Select(instruction => new CodeInstruction(instruction)).ToList();
    }
}
