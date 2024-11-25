namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class MeleeWeapon_CooldownReduction
{
    internal static ConditionalWeakTable<MeleeWeapon, NetFloat> Values { get; } = new();

    internal static NetFloat Get_CooldownReduction(this MeleeWeapon weapon)
    {
        return Values.GetOrCreateValue(weapon);
    }

    // Net types are readonly
    internal static void Set_CooldownReduction(this MeleeWeapon weapon, NetFloat value)
    {
    }
}
