namespace DaLion.Harmonics.Framework.Extensions;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using StardewValley.Buffs;

#endregion using directives

/// <summary>Extensions for the <see cref="BuffManager"/> class.</summary>
internal static class BuffManagerExtensions
{
    /// <summary>Gets the combined multiplier applied to the player's cooldown reduction.</summary>
    /// <param name="manager">The <see cref="BuffManager"/>.</param>
    /// <returns>The combined multiplier applied to the player's cooldown reduction.</returns>
    internal static float CooldownReduction(this BuffManager manager)
    {
        return manager.AppliedBuffs.Values.Sum(b => b.effects.Get_CooldownReduction().Value);
    }
}
