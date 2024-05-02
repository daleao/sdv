namespace DaLion.Core.Framework.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
internal static class StringExtensions
{
    /// <summary>Determines whether the number corresponds to a valid <see cref="Ring"/> index.</summary>
    /// <param name="id">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds any <see cref="Ring"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsRingId(this string id)
    {
        return int.Parse(id.AsSpan().Slice(id.IndexOf(')') + 1)) is >= 516 and <= 534 or 810 or 811;
    }
}
