namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    /// <summary>Converts the config-specified defensive swords into stabbing swords throughout the world.</summary>
    internal static void AddAllIntrinsicEnchantments()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon)
                {
                    weapon.AddIntrinsicEnchantments();
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon)
                {
                    weapon.AddIntrinsicEnchantments();
                }
            }
        }
    }

    /// <summary>Reverts all stabbing sword back into vanilla defensive swords.</summary>
    internal static void RemoveAllIntrinsicEnchantments()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon)
                {
                    weapon.RemoveIntrinsicEnchantments();
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon)
                {
                    weapon.RemoveIntrinsicEnchantments();
                }
            }
        }
    }

    /// <summary>Converts the config-specified defensive swords into stabbing swords throughout the world.</summary>
    internal static void ConvertAllStabbingSwords()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } sword)
                {
                    return;
                }

                if (Collections.StabbingSwords.Contains(sword.InitialParentTileIndex) ||
                    ArsenalModule.Config.Weapons.CustomStabbingSwords.Contains(sword.Name))
                {
                    sword.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon && weapon.type.Value == MeleeWeapon.defenseSword &&
                    (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex) ||
                     ArsenalModule.Config.Weapons.CustomStabbingSwords.Contains(weapon.Name)))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }
            }
        }
    }

    /// <summary>Reverts all stabbing sword back into vanilla defensive swords.</summary>
    internal static void RevertAllStabbingSwords()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                {
                    sword.type.Value = MeleeWeapon.defenseSword;
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                {
                    sword.type.Value = MeleeWeapon.defenseSword;
                }
            }
        }
    }

    /// <summary>Refreshes the stats of the all <see cref="MeleeWeapon"/>s in existence.</summary>
    /// <param name="option">The <see cref="RefreshOption"/>.</param>
    internal static void RefreshAllWeapons(RefreshOption option)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon)
                {
                    return;
                }

                weapon.RefreshStats(option);
                MeleeWeapon_Stats.Invalidate(weapon);
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is not MeleeWeapon weapon)
                {
                    continue;
                }

                weapon.RefreshStats(option);
                MeleeWeapon_Stats.Invalidate(weapon);
            }
        }
    }

    /// <summary>Transforms the currently held weapon into the Holy Blade.</summary>
    internal static void GetHolyBlade()
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex } darkSword)
        {
            return;
        }

        Game1.flashAlpha = 1f;
        player.holdUpItemThenMessage(new MeleeWeapon(Constants.HolyBladeIndex));
        darkSword.transform(Constants.HolyBladeIndex);
        darkSword.RefreshStats();
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}
