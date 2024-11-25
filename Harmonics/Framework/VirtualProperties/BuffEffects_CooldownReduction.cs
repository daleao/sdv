namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Buffs;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class BuffEffects_CooldownReduction
{
    internal static ConditionalWeakTable<BuffEffects, NetFloat> Values { get; } = new();

    internal static NetFloat Get_CooldownReduction(this BuffEffects effects)
    {
        return Values.GetOrCreateValue(effects);
    }

    // Net types are readonly
    internal static void Set_CooldownReduction(this BuffEffects effects, NetFloat value)
    {
    }
}
