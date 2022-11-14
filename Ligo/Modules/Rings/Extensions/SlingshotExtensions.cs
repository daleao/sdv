namespace DaLion.Ligo.Modules.Rings.Extensions;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
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
        slingshot.Write(DataFields.ResonantDamage, null);
        slingshot.Write(DataFields.ResonantCritChance, null);
        slingshot.Write(DataFields.ResonantCritPower, null);
        slingshot.Write(DataFields.ResonantKnockback, null);
        slingshot.Write(DataFields.ResonantCooldownReduction, null);
        slingshot.Write(DataFields.ResonantSpeed, null);
        slingshot.Write(DataFields.ResonantDefense, null);

        var player = Game1.player;
        var rings = Ligo.Integrations.WearMoreRingsApi?.GetAllRings(player) ??
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
                    slingshot.Increment(DataFields.ResonantDamage, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Aquamarine).Then(() =>
                    slingshot.Increment(DataFields.ResonantCritChance, enchantment.GetLevel() * 0.023f))
                .When(Gemstone.Amethyst).Then(() =>
                    slingshot.Increment(DataFields.ResonantKnockback, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Garnet).Then(() =>
                    slingshot.Increment(DataFields.ResonantCooldownReduction, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Emerald).Then(() =>
                    slingshot.Increment(DataFields.ResonantSpeed, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Jade).Then(() =>
                    slingshot.Increment(DataFields.ResonantCritPower, enchantment.GetLevel() * 0.25f))
                .When(Gemstone.Topaz).Then(() =>
                    slingshot.Increment(DataFields.ResonantDefense, enchantment.GetLevel() * 0.05f));
        }
    }

    /// <summary>Removes resonance bonuses applied to this <see cref="Slingshot"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    internal static void RemoveResonances(this Slingshot slingshot)
    {
        slingshot.Write(DataFields.ResonantDamage, null);
        slingshot.Write(DataFields.ResonantCritChance, null);
        slingshot.Write(DataFields.ResonantCritPower, null);
        slingshot.Write(DataFields.ResonantKnockback, null);
        slingshot.Write(DataFields.ResonantCooldownReduction, null);
        slingshot.Write(DataFields.ResonantSpeed, null);
        slingshot.Write(DataFields.ResonantDefense, null);
    }
}
