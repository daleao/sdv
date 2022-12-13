// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Overhaul.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Slingshot_Stats
{
    internal static ConditionalWeakTable<Slingshot, Holder> Values { get; } = new();

    internal static float Get_EffectiveDamageModifier(this Slingshot slingshot)
    {
        return 1f + Values.GetValue(slingshot, Create).Damage;
    }

    internal static float Get_RelativeDamageModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).Damage;
    }

    internal static float Get_EffectiveKnockbackModifer(this Slingshot slingshot)
    {
        return 1f + Values.GetValue(slingshot, Create).Knockback;
    }

    internal static float Get_RelativeKnockbackModifer(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).Knockback;
    }

    internal static float Get_EffectiveCritChanceModifier(this Slingshot slingshot)
    {
        return 1f + Values.GetValue(slingshot, Create).CritChance;
    }

    internal static float Get_RelativeCritChanceModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).CritChance;
    }

    internal static float Get_EffectiveCritPowerModifier(this Slingshot slingshot)
    {
        return 1f + Values.GetValue(slingshot, Create).CritPower;
    }

    internal static float Get_RelativeCritPowerModifier(this Slingshot slingshot)
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

        if (slingshot.hasEnchantmentOfType<RubyEnchantment>())
        {
            holder.Damage = slingshot.GetEnchantmentLevel<RubyEnchantment>() * 0.1f;
        }

        if (slingshot.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
        {
            holder.Damage += (float)rubyChord.Amplitude * 0.1f;
        }

        if (slingshot.hasEnchantmentOfType<AmethystEnchantment>())
        {
            holder.Knockback = slingshot.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f;
        }

        if (slingshot.Get_ResonatingChord<AmethystEnchantment>() is { } amethystChord)
        {
            holder.Knockback += (float)amethystChord.Amplitude * 0.1f;
        }

        if (slingshot.hasEnchantmentOfType<AquamarineEnchantment>())
        {
            holder.CritChance = slingshot.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f;
        }

        if (slingshot.Get_ResonatingChord<AquamarineEnchantment>() is { } aquamarineChord)
        {
            holder.CritChance += (float)aquamarineChord.Amplitude * 0.046f;
        }

        if (slingshot.hasEnchantmentOfType<JadeEnchantment>())
        {
            holder.CritPower = slingshot.GetEnchantmentLevel<JadeEnchantment>() *
                               (ArsenalModule.Config.RebalancedForges ? 0.5f : 0.1f);
        }

        if (slingshot.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
        {
            holder.CritPower += (float)jadeChord.Amplitude * (ArsenalModule.Config.RebalancedForges ? 0.5f : 0.1f);
        }

        if (slingshot.hasEnchantmentOfType<EmeraldEnchantment>())
        {
            holder.FireSpeed = slingshot.GetEnchantmentLevel<EmeraldEnchantment>();
        }

        if (slingshot.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
        {
            holder.FireSpeed += (float)emeraldChord.Amplitude;
        }

        if (slingshot.hasEnchantmentOfType<GarnetEnchantment>())
        {
            holder.CooldownReduction = slingshot.GetEnchantmentLevel<GarnetEnchantment>();
        }

        if (slingshot.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
        {
            holder.CooldownReduction += (float)garnetChord.Amplitude;
        }

        if (slingshot.hasEnchantmentOfType<TopazEnchantment>())
        {
            holder.Resilience = slingshot.GetEnchantmentLevel<TopazEnchantment>();
        }

        if (slingshot.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
        {
            holder.Resilience += (float)topazChord.Amplitude;
        }

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
