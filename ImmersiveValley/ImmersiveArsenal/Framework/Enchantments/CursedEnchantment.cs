namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Common.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Dark Sword.</summary>
public class CursedEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment() => true;

    public override bool IsForge() => false;

    public override int GetMaximumLevel() => 1;

    public override bool ShouldBeDisplayed() => false;

    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        who.health = Math.Max(who.health - (int)(who.maxHealth * 0.01f), 0);
    }

    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        var sword = who.CurrentTool as MeleeWeapon;
        if (sword?.hasEnchantmentOfType<CursedEnchantment>() != true)
            ThrowHelper.ThrowInvalidOperationException("Current tool does not have Demonic Enchantment");

        sword.Increment("EnemiesSlain");
    }
}