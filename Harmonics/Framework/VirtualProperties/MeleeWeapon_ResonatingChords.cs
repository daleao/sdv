namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class MeleeWeapon_ResonatingChords
{
    internal static ConditionalWeakTable<MeleeWeapon, List<Chord>> Values { get; } = new();

    internal static List<Chord> Get_ResonatingChords(this MeleeWeapon weapon)
    {
        return Values.GetOrCreateValue(weapon);
    }
}
