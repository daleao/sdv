namespace DaLion.Stardew.FishPonds.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

internal static class Utility
{
    /// <summary>Dictionary of extended family pair by legendary fish id.</summary>
    internal static readonly Dictionary<int, int> ExtendedFamilyPairs = new()
    {
        { 159, 898 },
        { 160, 899 },
        { 163, 900 },
        { 682, 901 },
        { 775, 902 },
        { 898, 159 },
        { 899, 160 },
        { 900, 163 },
        { 901, 682 },
        { 902, 775 }
    };

    /// <summary>Whether the currently held fish is a family member of another.</summary>
    /// <param name="held">The index of the currently held fish.</param>
    /// <param name="other">The index of some other fish.</param>
    /// <returns></returns>
    internal static bool IsExtendedFamilyMember(int held, int other)
    {
        return ExtendedFamilyPairs.TryGetValue(other, out var pair) && pair == held;
    }
}