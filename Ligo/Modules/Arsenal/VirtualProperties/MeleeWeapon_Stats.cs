// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class MeleeWeapon_Stats
{
    internal static ConditionalWeakTable<MeleeWeapon, Holder> Values { get; } = new();

    internal static int Get_MinDamage(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).MinDamage;
    }

    internal static int Get_MaxDamage(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).MaxDamage;
    }

    internal static float Get_AbsoluteKnockback(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Knockback;
    }

    internal static float Get_RelativeKnockback(this MeleeWeapon weapon)
    {
        var knockback = Values.GetValue(weapon, Create).Knockback;
        var @default = weapon.defaultKnockBackForThisType(weapon.type.Value);
        if (knockback == @default)
        {
            return 0f;
        }

        return (knockback / @default) - 1f;
    }

    internal static float Get_EffectiveCritChance(this MeleeWeapon weapon)
    {
        var critChance = Values.GetValue(weapon, Create).CritChance;
        return weapon.type.Value != MeleeWeapon.dagger ? critChance : (critChance + 0.005f) * 1.12f;
    }

    internal static float Get_RelativeCritChance(this MeleeWeapon weapon)
    {
        var critChance = Values.GetValue(weapon, Create).CritChance;
        var @default = weapon.DefaultCritChance();
        if (critChance == @default)
        {
            return 0f;
        }

        return (critChance / @default) - 1f;
    }

    internal static float Get_EffectiveCritPower(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).CritChance;
    }

    internal static float Get_RelativeCritPower(this MeleeWeapon weapon)
    {
        var critPower = Values.GetValue(weapon, Create).CritPower;
        var @default = weapon.DefaultCritPower();
        if (critPower == @default)
        {
            return 0f;
        }

        return (critPower / @default) - 1f;
    }

    internal static float Get_EffectiveSwingSpeed(this MeleeWeapon weapon)
    {
        return 10f / (10f + Values.GetValue(weapon, Create).SwingSpeed);
    }

    internal static float Get_RelativeSwingSpeed(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).SwingSpeed * 0.1f;
    }

    internal static float Get_EffectiveCooldownReduction(this MeleeWeapon weapon)
    {
        return 1f - (Values.GetValue(weapon, Create).CooldownReduction * 0.1f);
    }

    internal static float Get_RelativeCooldownReduction(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).CooldownReduction * 0.1f;
    }

    internal static float Get_EffectiveResilience(this MeleeWeapon weapon)
    {
        return 10f / (10f + Values.GetValue(weapon, Create).Resilience);
    }

    internal static float Get_RelativeResilience(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Resilience * 0.1f;
    }

    internal static int Count_NonZeroStats(this MeleeWeapon weapon)
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

    internal static int Get_Level(this MeleeWeapon weapon)
    {
        return Values.GetValue(weapon, Create).Level;
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
        if (weapon.hasEnchantmentOfType<CursedEnchantment>())
        {
            var curseBonus = weapon.Read<int>(DataFields.CursePoints) / 25;
            holder.MinDamage += curseBonus;
            holder.MaxDamage += curseBonus;
        }

        if (weapon.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
        {
            holder.MinDamage = (int)(holder.MinDamage + (weapon.Read<int>(DataFields.BaseMinDamage) *
                                                         weapon.GetEnchantmentLevel<RubyEnchantment>() *
                                                         rubyChord.Amplitude * 0.1f));
            holder.MaxDamage = (int)(holder.MaxDamage + (weapon.Read<int>(DataFields.BaseMaxDamage) *
                                                         weapon.GetEnchantmentLevel<RubyEnchantment>() *
                                                         rubyChord.Amplitude * 0.1f));
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
                (float)(weapon.GetEnchantmentLevel<AmethystEnchantment>() * aquamarineChord.Amplitude * 0.046f);
        }

        holder.CritPower = weapon.critMultiplier.Value;
        if (weapon.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
        {
            holder.CritPower += (float)(weapon.GetEnchantmentLevel<AmethystEnchantment>() * jadeChord.Amplitude *
                                        (ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f));
        }

        holder.SwingSpeed = weapon.speed.Value;
        if (weapon.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
        {
            holder.SwingSpeed += (float)(weapon.GetEnchantmentLevel<AmethystEnchantment>() * emeraldChord.Amplitude);
        }

        if (weapon.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
        {
            holder.CooldownReduction = (float)(weapon.GetEnchantmentLevel<GarnetEnchantment>() * garnetChord.Amplitude);
        }

        holder.Resilience = weapon.addedDefense.Value;
        if (weapon.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
        {
             holder.Resilience += (float)(weapon.GetEnchantmentLevel<TopazEnchantment>() * topazChord.Amplitude);
        }

        var points = weapon.Read<int>(DataFields.BaseMaxDamage) * weapon.type.Value switch
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

        if (weapon.IsUnique() || weapon.CanBeCrafted())
        {
            holder.Level++;
        }
        else if (holder.Level == 0)
        {
            holder.Level = 1;
        }

        holder.Level = Math.Min((int)Math.Floor(points / 10f), 10);
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
