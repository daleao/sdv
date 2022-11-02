namespace DaLion.Redux.Framework.Rings.Extensions;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
internal static class SlingshotExtensions
{
    /// <summary>Recalculates resonance bonuses applied to this <see cref="Slingshot"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    internal static void RecalculateResonances(this Slingshot slingshot)
    {
        slingshot.Write(DataFields.ResonantSlingshotDamage, null);
        slingshot.Write(DataFields.ResonantSlingshotCritChance, null);
        slingshot.Write(DataFields.ResonantSlingshotCritPower, null);
        slingshot.Write(DataFields.ResonantSlingshotKnockback, null);
        slingshot.Write(DataFields.ResonantSlingshotCooldownReduction, null);
        slingshot.Write(DataFields.ResonantSlingshotSpeed, null);
        slingshot.Write(DataFields.ResonantSlingshotDefense, null);

        var player = Game1.player;
        var rings = Framework.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
                    player.leftRing.Value.Collect(player.rightRing.Value);
        foreach (var ring in rings.OfType<CombinedRing>())
        {
            var chord = ring.Get_Chord();
            if (chord?.Root is null)
            {
                continue;
            }

            var enchantment =
                slingshot.enchantments.FirstOrDefault(e => e.GetType() == chord.Root.EnchantmentType);
            if (enchantment is null)
            {
                continue;
            }

            chord.Root
                .When(Gemstone.Ruby).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotDamage, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Aquamarine).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotCritChance, enchantment.GetLevel() * 0.023f))
                .When(Gemstone.Amethyst).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotCritPower, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Garnet).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotCooldownReduction, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Emerald).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotSpeed, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Jade).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotCritPower, enchantment.GetLevel() * 0.25f))
                .When(Gemstone.Topaz).Then(() =>
                    slingshot.Increment(DataFields.ResonantSlingshotDefense, enchantment.GetLevel() * 0.05f));
        }
    }

    /// <summary>Removes resonance bonuses applied to this <see cref="Slingshot"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    internal static void RemoveResonances(this Slingshot slingshot)
    {
        slingshot.Write(DataFields.ResonantSlingshotDamage, null);
        slingshot.Write(DataFields.ResonantSlingshotCritChance, null);
        slingshot.Write(DataFields.ResonantSlingshotCritPower, null);
        slingshot.Write(DataFields.ResonantSlingshotKnockback, null);
        slingshot.Write(DataFields.ResonantSlingshotCooldownReduction, null);
        slingshot.Write(DataFields.ResonantSlingshotSpeed, null);
        slingshot.Write(DataFields.ResonantSlingshotDefense, null);
    }
}
