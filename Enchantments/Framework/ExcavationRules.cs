namespace DaLion.Enchantments.Framework;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Enums;

#endregion using directive

/// <summary>Used to cache and retreive excavation rules for every possible tile mask.</summary>
internal static class ExcavationRules
{
    private static readonly Dictionary<(Direction Direction, string Mask), ExcavationResult> Rules = [];

    /// <summary>Attempts to fetch a set of <see cref="ExcavationRules"/> for the given player <paramref name="direction"/> and <paramref name="mask"/>.</summary>
    /// <param name="direction">The player's facing <see cref="Direction"/>.</param>
    /// <param name="mask">A mask representing <see cref="MapLayer.Buildings"/> tiles around the excavated target.</param>
    /// <param name="result">The corresponding <see cref="ExcavationResult"/>, if any.</param>
    /// <returns><see langword="true"/> if a corresponding <see cref="ExcavationResult"/> exists, otherwise <see langword="false"/>.</returns>
    public static bool TryGet(
        Direction direction,
        string mask,
        [NotNullWhen(true)] out ExcavationResult? result)
    {
        return Rules.TryGetValue((direction, mask), out result);
    }
}
