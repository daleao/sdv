namespace DaLion.Ligo.Modules.Rings.Extensions;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
internal static class MeleeWeaponExtensions
{
    /// <summary>Recalculates resonance bonuses applied to this <see cref="MeleeWeapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RecalculateResonances(this MeleeWeapon weapon)
    {
        weapon.Write(DataFields.ResonantDamage, null);
        weapon.Write(DataFields.ResonantKnockback, null);
        weapon.Write(DataFields.ResonantCritChance, null);
        weapon.Write(DataFields.ResonantCritPower, null);
        weapon.Write(DataFields.ResonantSpeed, null);
        weapon.Write(DataFields.ResonantCooldownReduction, null);
        weapon.Write(DataFields.ResonantResilience, null);

        var player = Game1.player;
        foreach (var chord in player.Get_ResonatingChords())
        {
            if (chord.Root is null)
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
                    weapon.Increment(DataFields.ResonantDamage, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Aquamarine).Then(() =>
                    weapon.Increment(DataFields.ResonantCritChance, enchantment.GetLevel() * 0.023f))
                .When(Gemstone.Amethyst).Then(() =>
                    weapon.Increment(DataFields.ResonantKnockback, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Garnet).Then(() =>
                    weapon.Increment(DataFields.ResonantCooldownReduction, enchantment.GetLevel() * 0.05f))
                .When(Gemstone.Emerald).Then(() =>
                    weapon.Increment(DataFields.ResonantSpeed, enchantment.GetLevel() * 0.5f))
                .When(Gemstone.Jade).Then(() =>
                    weapon.Increment(DataFields.ResonantCritPower, enchantment.GetLevel() * 0.25f))
                .When(Gemstone.Topaz).Then(() =>
                    weapon.Increment(DataFields.ResonantResilience, enchantment.GetLevel() * 0.5f));
        }

        if (ModEntry.Config.EnableArsenal)
        {
            weapon.Invalidate();
        }
    }

    /// <summary>Removes resonance bonuses applied to this <see cref="MeleeWeapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RemoveResonances(this MeleeWeapon weapon)
    {
        weapon.Write(DataFields.ResonantDamage, null);
        weapon.Write(DataFields.ResonantCritChance, null);
        weapon.Write(DataFields.ResonantCritPower, null);
        weapon.Write(DataFields.ResonantKnockback, null);
        weapon.Write(DataFields.ResonantCooldownReduction, null);
        weapon.Write(DataFields.ResonantSpeed, null);
        weapon.Write(DataFields.ResonantResilience, null);
        if (ModEntry.Config.EnableArsenal)
        {
            weapon.Invalidate();
        }
    }
}
