﻿namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
public class BlessedEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }
}