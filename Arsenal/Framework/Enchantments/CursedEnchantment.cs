﻿namespace DaLion.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Arsenal.Framework.Events.GameLoop.OneSecondUpdateTicked;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Dark Sword.</summary>
[XmlType("Mods_DaLion_CursedEnchantment")]
public sealed class CursedEnchantment : BaseWeaponEnchantment
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
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.mailReceived.Contains("gotDarkSword"))
        {
            who.mailReceived.Add("gotDarkSword");
        }

        EventManager.Enable<CurseOneSecondUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        EventManager.Disable<CurseOneSecondUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        base._OnMonsterSlay(m, location, who);
        if (who.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword } darkSword)
        {
            return;
        }

        darkSword.Increment(DataKeys.CursePoints);
        if (darkSword.Read<int>(DataKeys.CursePoints) >= 50 && !who.hasOrWillReceiveMail("viegoCurse"))
        {
            who.mailForTomorrow.Add("viegoCurse");
        }
    }
}
