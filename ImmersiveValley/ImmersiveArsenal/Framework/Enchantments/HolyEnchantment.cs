namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Monsters;
using System;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
public class HolyEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment() => true;

    public override bool IsForge() => false;

    public override int GetMaximumLevel() => 1;

    public override bool ShouldBeDisplayed() => false;

    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        who.health = Math.Max(who.health - (int)(who.maxHealth * 0.01f), 0);
    }
}