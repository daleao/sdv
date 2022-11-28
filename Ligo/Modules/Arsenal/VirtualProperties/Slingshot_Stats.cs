// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Slingshot_Stats
{
    internal static ConditionalWeakTable<Slingshot, Holder> Values { get; } = new();

    internal static float Get_DamageModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).Damage;
    }

    internal static float Get_KnockbackModifer(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).Knockback;
    }

    internal static float Get_CritChanceModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).CritChance;
    }

    internal static float Get_CritPowerModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).CritPower;
    }

    internal static float Get_EffectiveFireSpeed(this Slingshot slingshot)
    {
        return 10f / (10f + Values.GetValue(slingshot, Create).FireSpeed);
    }

    internal static float Get_RelativeFireSpeed(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).FireSpeed * 0.1f;
    }

    internal static float Get_EffectiveCooldownReduction(this Slingshot slingshot)
    {
        return 1f - (Values.GetValue(slingshot, Create).CooldownReduction * 0.1f);
    }

    internal static float Get_RelativeCooldownReduction(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).CooldownReduction * 0.1f;
    }

    internal static float Get_EffectiveResilience(this Slingshot slingshot)
    {
        return 10f / (10f + Values.GetValue(slingshot, Create).Resilience);
    }

    internal static float Get_RelativeResilience(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).Resilience * 0.1f;
    }

    internal static void Invalidate(this Slingshot slingshot)
    {
        Values.Remove(slingshot);
    }

    private static Holder Create(Slingshot slingshot)
    {
        var holder = new Holder();

        holder.Damage = (slingshot.GetEnchantmentLevel<RubyEnchantment>() * 0.1f) +
                        slingshot.Read<float>(DataFields.ResonantDamage);
        holder.Knockback = (slingshot.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f) +
                           slingshot.Read<float>(DataFields.ResonantKnockback);
        holder.CritChance = (slingshot.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f) +
                            slingshot.Read<float>(DataFields.ResonantCritChance);
        holder.CritPower = (slingshot.GetEnchantmentLevel<JadeEnchantment>() * (ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f)) +
                           slingshot.Read<float>(DataFields.ResonantCritChance);
        holder.FireSpeed = slingshot.GetEnchantmentLevel<EmeraldEnchantment>() +
                           slingshot.Read<float>(DataFields.ResonantSpeed);
        holder.CooldownReduction = slingshot.GetEnchantmentLevel<GarnetEnchantment>() +
                            slingshot.Read<float>(DataFields.ResonantCooldownReduction);
        holder.Resilience = slingshot.GetEnchantmentLevel<TopazEnchantment>() +
                            slingshot.Read<float>(DataFields.ResonantResilience);

        return holder;
    }

    internal class Holder
    {
        public float Damage { get; internal set; }

        public float Knockback { get; internal set; }

        public float CritChance { get; internal set; }

        public float CritPower { get; internal set; }

        public float FireSpeed { get; internal set; }

        public float CooldownReduction { get; internal set; }

        public float Resilience { get; internal set; }
    }
}
