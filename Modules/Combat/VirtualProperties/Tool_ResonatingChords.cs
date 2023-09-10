namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Tool_ResonatingChords
{
    internal static ConditionalWeakTable<Tool, Dictionary<Type, Chord>> Values { get; } = new();

    internal static Chord? Get_ResonatingChord<TEnchantment>(this Tool tool)
        where TEnchantment : BaseWeaponEnchantment
    {
        return CombatModule.Config.EnableResonances && Values.TryGetValue(tool, out var dict) &&
               dict.TryGetValue(typeof(TEnchantment), out var chord)
            ? chord
            : null;
    }

    internal static Chord? Get_ResonatingChord(this Tool tool, Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseWeaponEnchantment)))
        {
            ThrowHelper.ThrowInvalidOperationException($"Tried to get the resonating chord for non-enchantment type {type}");
        }

        return CombatModule.Config.EnableResonances && Values.TryGetValue(tool, out var dict) &&
               dict.TryGetValue(type, out var chord)
            ? chord
            : null;
    }

    internal static void UpdateResonatingChord<TEnchantment>(this Tool tool, Chord newValue)
        where TEnchantment : BaseWeaponEnchantment
    {
        if (newValue.Root?.EnchantmentType != typeof(TEnchantment))
        {
            ThrowHelper.ThrowInvalidOperationException($"Type mismatch between {newValue} and {typeof(TEnchantment)}.");
        }

        var dict = Values.GetOrCreateValue(tool);
        if (dict.TryGetValue(typeof(TEnchantment), out var oldValue) && oldValue.Amplitude > newValue.Amplitude)
        {
            return;
        }

        dict[typeof(TEnchantment)] = newValue;
        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    internal static void UpdateResonatingChord(this Tool tool, Chord newValue)
    {
        var dict = Values.GetOrCreateValue(tool);
        if (dict.TryGetValue(newValue.Root!.EnchantmentType, out var oldValue) && oldValue.Amplitude > newValue.Amplitude)
        {
            return;
        }

        dict[newValue.Root.EnchantmentType] = newValue;
        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    internal static void UnsetResonatingChord<TEnchantment>(this Tool tool)
        where TEnchantment : BaseWeaponEnchantment
    {
        if (!Values.TryGetValue(tool, out var dict))
        {
            return;
        }

        if (!dict.Remove(typeof(TEnchantment)))
        {
            return;
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    internal static void UnsetResonatingChord(this Tool tool, Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseWeaponEnchantment)))
        {
            ThrowHelper.ThrowInvalidOperationException($"Tried to unset the resonating chord of non-enchantment type {type}");
        }

        if (!Values.TryGetValue(tool, out var dict))
        {
            return;
        }

        if (!dict.Remove(type))
        {
            return;
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    internal static void UnsetAllResonatingChords(this Tool tool)
    {
        if (!Values.Remove(tool))
        {
            return;
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }
}
