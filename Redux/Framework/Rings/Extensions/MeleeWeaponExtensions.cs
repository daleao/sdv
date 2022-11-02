namespace DaLion.Redux.Framework.Rings.Extensions;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
internal static class MeleeWeaponExtensions
{
    /// <summary>Recalculates resonance bonuses applied to this <see cref="MeleeWeapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RecalculateResonances(this MeleeWeapon weapon)
    {
        weapon.Write(DataFields.ResonantWeaponDamage, null);
        weapon.Write(DataFields.ResonantWeaponCritChance, null);
        weapon.Write(DataFields.ResonantWeaponCritPower, null);
        weapon.Write(DataFields.ResonantWeaponKnockback, null);
        weapon.Write(DataFields.ResonantWeaponCooldownReduction, null);
        weapon.Write(DataFields.ResonantWeaponSpeed, null);
        weapon.Write(DataFields.ResonantWeaponDefense, null);

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
                weapon.enchantments.FirstOrDefault(e => e.GetType() == chord.Root.EnchantmentType);
            if (enchantment is null)
            {
                continue;
            }

            chord.Root
                .When(Gemstone.Ruby).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponDamage, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Aquamarine).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponCritChance, enchantment.GetLevel() * 0.023f))
                .When(Gemstone.Amethyst).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponCritPower, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Garnet).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponCooldownReduction, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Emerald).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponSpeed, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Jade).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponCritPower, enchantment.GetLevel() * 0.25f))
                .When(Gemstone.Topaz).Then(() =>
                    weapon.Increment(DataFields.ResonantWeaponDefense, enchantment.GetLevel() * 0.05f));
        }
    }

    /// <summary>Removes resonance bonuses applied to this <see cref="MeleeWeapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RemoveResonances(this MeleeWeapon weapon)
    {
        weapon.Write(DataFields.ResonantWeaponDamage, null);
        weapon.Write(DataFields.ResonantWeaponCritChance, null);
        weapon.Write(DataFields.ResonantWeaponCritPower, null);
        weapon.Write(DataFields.ResonantWeaponKnockback, null);
        weapon.Write(DataFields.ResonantWeaponCooldownReduction, null);
        weapon.Write(DataFields.ResonantWeaponSpeed, null);
        weapon.Write(DataFields.ResonantWeaponDefense, null);
    }
}
