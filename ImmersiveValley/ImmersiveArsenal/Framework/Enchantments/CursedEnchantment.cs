namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System;
using CommunityToolkit.Diagnostics;
using DaLion.Common.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Dark Sword.</summary>
public class CursedEnchantment : BaseWeaponEnchantment
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

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        who.health = Math.Max(who.health - (int)(who.maxHealth * 0.01f), 0);
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        var sword = who.CurrentTool as MeleeWeapon;
        if (sword?.hasEnchantmentOfType<CursedEnchantment>() != true)
        {
            ThrowHelper.ThrowInvalidOperationException("Current tool does not have Demonic Enchantment");
        }

        sword.Increment("EnemiesSlain");
    }
}
