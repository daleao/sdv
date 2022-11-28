// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Extensions;
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
        return Values.GetValue(weapon, Create).Knockback - weapon.defaultKnockBackForThisType(weapon.type.Value);
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

        return (critChance > @default ? critChance / @default : -@default / critChance) - 1f;
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

        return (critPower > @default ? critPower / @default : -@default / critPower) - 1f;
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

        holder.MinDamage = (int)(holder.MinDamage * (1f + weapon.Read<float>(DataFields.ResonantDamage)));
        holder.MaxDamage = (int)(holder.MaxDamage * (1f + weapon.Read<float>(DataFields.ResonantDamage)));

        holder.Knockback = weapon.knockback.Value * (1f + weapon.Read<float>(DataFields.ResonantKnockback));
        holder.CritChance = weapon.critChance.Value * (1f + weapon.Read<float>(DataFields.ResonantCritChance));
        holder.CritPower = weapon.critMultiplier.Value * (1f + weapon.Read<float>(DataFields.ResonantCritPower));

        holder.SwingSpeed = weapon.speed.Value + weapon.Read<float>(DataFields.ResonantSpeed);
        holder.Resilience = weapon.addedDefense.Value + weapon.Read<float>(DataFields.ResonantResilience);
        holder.CooldownReduction = weapon.GetEnchantmentLevel<GarnetEnchantment>() +
                            weapon.Read<float>(DataFields.ResonantCooldownReduction);

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
    }
}
