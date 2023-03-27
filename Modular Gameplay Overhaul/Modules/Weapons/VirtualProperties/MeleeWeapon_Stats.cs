// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Overhaul.Modules.Weapons.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class MeleeWeapon_Stats
{
    internal static ConditionalWeakTable<MeleeWeapon, Holder> Values { get; } = new();

    internal static int Get_MinDamage(this MeleeWeapon weapon)
    {
        if (weapon.InitialParentTileIndex == ItemIDs.InsectHead && WeaponsModule.Config.EnableRebalance)
        {
            var caveInsectsKilled = Game1.stats.getMonstersKilled("Grub") +
                                    Game1.stats.getMonstersKilled("Fly") +
                                    Game1.stats.getMonstersKilled("Bug");
            // ReSharper disable once PossibleLossOfFraction
            return (int)(caveInsectsKilled / 5 * 0.85);
        }

        var minDamage = Values.GetValue(weapon, Create).MinDamage;
        if (weapon.hasEnchantmentOfType<CursedEnchantment>())
        {
            minDamage += weapon.Read<int>(DataKeys.CursePoints) / 5;
        }

        return minDamage;
    }

    internal static int Get_MaxDamage(this MeleeWeapon weapon)
    {
        if (weapon.InitialParentTileIndex == ItemIDs.InsectHead && WeaponsModule.Config.EnableRebalance)
        {
            var caveInsectsKilled = Game1.stats.getMonstersKilled("Grub") +
                                    Game1.stats.getMonstersKilled("Fly") +
                                    Game1.stats.getMonstersKilled("Bug");
            return caveInsectsKilled / 5;
        }

        var maxDamage = Values.GetValue(weapon, Create).MaxDamage;
        if (weapon.hasEnchantmentOfType<CursedEnchantment>())
        {
            maxDamage += weapon.Read<int>(DataKeys.CursePoints) / 5;
        }

        return maxDamage;
    }

    internal static float Get_EffectiveKnockback(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Knockback;
    }

    internal static float Get_DisplayKnockback(this MeleeWeapon weapon)
    {
        var knockback = Values.GetValue(weapon, Create).Knockback;
        var @default = weapon.defaultKnockBackForThisType(weapon.type.Value);
        if (knockback == @default)
        {
            return 0f;
        }

        switch (WeaponsModule.Config.WeaponTooltipStyle)
        {
            case Config.TooltipStyle.Absolute:
                return knockback - @default;
            case Config.TooltipStyle.Relative:
                return (knockback / @default) - 1f;
            default:
                return ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Config.TooltipStyle, float>(
                    WeaponsModule.Config.WeaponTooltipStyle);
        }
    }

    internal static float Get_EffectiveCritChance(this MeleeWeapon weapon)
    {
        var critChance = Values.GetValue(weapon, Create).CritChance;
        return weapon.type.Value != MeleeWeapon.dagger ? critChance : (critChance + 0.005f) * 1.12f;
    }

    internal static float Get_DisplayCritChance(this MeleeWeapon weapon)
    {
        var critChance = Values.GetValue(weapon, Create).CritChance;
        var @default = weapon.DefaultCritChance();
        if (critChance == @default)
        {
            return 0f;
        }

        switch (WeaponsModule.Config.WeaponTooltipStyle)
        {
            case Config.TooltipStyle.Absolute:
                return critChance - @default;
            case Config.TooltipStyle.Relative:
                return (critChance / @default) - 1f;
            default:
                return ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Config.TooltipStyle, float>(
                    WeaponsModule.Config.WeaponTooltipStyle);
        }
    }

    internal static float Get_EffectiveCritPower(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).CritPower;
    }

    internal static float Get_DisplayCritPower(this MeleeWeapon weapon)
    {
        var critPower = Values.GetValue(weapon, Create).CritPower;
        var @default = weapon.DefaultCritPower();
        if (critPower == @default)
        {
            return 0f;
        }

        switch (WeaponsModule.Config.WeaponTooltipStyle)
        {
            case Config.TooltipStyle.Absolute:
                return critPower - @default;
            case Config.TooltipStyle.Relative:
                return (critPower / @default) - 1f;
            default:
                return ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Config.TooltipStyle, float>(
                    WeaponsModule.Config.WeaponTooltipStyle);
        }
    }

    internal static float Get_EffectiveSwingSpeed(this MeleeWeapon weapon)
    {
        return 10f / (10f + Values.GetValue(weapon, Create).SwingSpeed);
    }

    internal static float Get_DisplaySwingSpeed(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).SwingSpeed * 0.1f;
    }

    internal static float Get_EffectiveCooldownReduction(this MeleeWeapon weapon)
    {
        return 1f - (Values.GetValue(weapon, Create).CooldownReduction * 0.1f);
    }

    internal static float Get_DisplayCooldownReduction(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).CooldownReduction * 0.1f;
    }

    internal static float Get_EffectiveResilience(this MeleeWeapon weapon)
    {
        return 10f / (10f + Values.GetValue(weapon, Create).Resilience);
    }

    internal static float Get_DisplayResilience(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Resilience * 0.1f;
    }

    internal static int Get_Level(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Level;
    }

    internal static int CountNonZeroStats(this MeleeWeapon weapon)
    {
        var count = 1;

        if (Values.GetValue(weapon, Create).Knockback != weapon.defaultKnockBackForThisType(weapon.type.Value))
        {
            count++;
        }

        if (Values.GetValue(weapon, Create).CritChance != weapon.DefaultCritChance())
        {
            count++;
        }

        if (Values.GetValue(weapon, Create).CritPower != weapon.DefaultCritPower())
        {
            count++;
        }

        if (Values.GetValue(weapon, Create).SwingSpeed != 0)
        {
            count++;
        }

        if (Values.GetValue(weapon, Create).CooldownReduction > 0)
        {
            count++;
        }

        if (Values.GetValue(weapon, Create).Resilience != 0)
        {
            count++;
        }

        return count;
    }

    internal static void Invalidate(this MeleeWeapon weapon)
    {
        Values.Remove(weapon);
    }

    private static Holder Create(MeleeWeapon weapon)
    {
        var holder = new Holder();

        holder.MinDamage = weapon.minDamage.Value;
        holder.MaxDamage = weapon.maxDamage.Value;
        var data = ModHelper.GameContent
            .Load<Dictionary<int, string>>("Data/weapons")[weapon.InitialParentTileIndex]
            .SplitWithoutAllocation('/');
        if (weapon.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
        {
            holder.MinDamage = (int)(holder.MinDamage +
                                     (weapon.Read(DataKeys.BaseMinDamage, int.Parse(data[2])) *
                                      weapon.GetEnchantmentLevel<RubyEnchantment>() * rubyChord.Amplitude * 0.1f));
            holder.MaxDamage = (int)(holder.MaxDamage +
                                     (weapon.Read(DataKeys.BaseMaxDamage, int.Parse(data[3])) *
                                      weapon.GetEnchantmentLevel<RubyEnchantment>() * rubyChord.Amplitude * 0.1f));
        }

        holder.Knockback = weapon.knockback.Value;
        if (weapon.Get_ResonatingChord<AmethystEnchantment>() is { } amethystChord)
        {
            holder.Knockback +=
                (float)(weapon.GetEnchantmentLevel<AmethystEnchantment>() * amethystChord.Amplitude * 0.1f);
        }

        holder.CritChance = weapon.critChance.Value;
        if (weapon.Get_ResonatingChord<AquamarineEnchantment>() is { } aquamarineChord)
        {
            holder.CritChance +=
                (float)(weapon.GetEnchantmentLevel<AquamarineEnchantment>() * aquamarineChord.Amplitude * 0.046f);
        }

        holder.CritPower = weapon.critMultiplier.Value;
        if (weapon.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
        {
            holder.CritPower += (float)(weapon.GetEnchantmentLevel<JadeEnchantment>() * jadeChord.Amplitude *
                                        (EnchantmentsModule.IsEnabled && EnchantmentsModule.Config.RebalancedForges ? 0.5f : 0.1f));
        }

        holder.SwingSpeed = weapon.speed.Value;
        if (weapon.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
        {
            holder.SwingSpeed += (float)(weapon.GetEnchantmentLevel<EmeraldEnchantment>() * emeraldChord.Amplitude);
        }

        holder.CooldownReduction = weapon.GetEnchantmentLevel<GarnetEnchantment>();
        if (weapon.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
        {
            holder.CooldownReduction += (float)(weapon.GetEnchantmentLevel<GarnetEnchantment>() * garnetChord.Amplitude);
        }

        holder.Resilience = weapon.addedDefense.Value;
        if (weapon.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
        {
             holder.Resilience += (float)(weapon.GetEnchantmentLevel<TopazEnchantment>() * topazChord.Amplitude);
        }

        var points = weapon.Read(DataKeys.BaseMaxDamage, int.Parse(data[3])) * weapon.type.Value switch
        {
            MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword => 0.5f,
            MeleeWeapon.dagger => 0.75f,
            MeleeWeapon.club => 0.3f,
            _ => 0f,
        };

        points += (weapon.knockback.Value - weapon.defaultKnockBackForThisType(weapon.type.Value)) *
                  10f;
        points += ((weapon.critChance.Value / weapon.DefaultCritChance()) - 1f) * 10f;
        points += ((weapon.critMultiplier.Value / weapon.DefaultCritPower()) - 1f) * 10f;
        points += weapon.addedPrecision.Value;
        points += weapon.addedDefense.Value;
        points += weapon.speed.Value;
        points += weapon.addedAreaOfEffect.Value / 4f;

        holder.Level = (int)Math.Floor(points / 10f);
        if (weapon.IsUnique() || weapon.CanBeCrafted())
        {
            holder.Level++;
        }

        holder.Level = Math.Clamp(holder.Level, 1, 10);
        return holder;
    }

    internal class Holder
    {
        public int MinDamage { get; internal set; }

        public int MaxDamage { get; internal set; }

        public float Knockback { get; internal set; }

        public float CritChance { get; internal set; }

        public float CritPower { get; internal set; }

        public float SwingSpeed { get; internal set; }

        public float CooldownReduction { get; internal set; }

        public float Resilience { get; internal set; }

        public int Level { get; internal set; }
    }
}
