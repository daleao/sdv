namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley;
using StardewValley.Monsters;
using System;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
public class HolyEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    public override bool IsForge()
    {
        return false;
    }

    public override int GetMaximumLevel()
    {
        return 1;
    }

    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        who.health = Math.Max(who.health - (int)(who.maxHealth * 0.01f), 0);
    }
}