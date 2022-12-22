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
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                weapon.AddIntrinsicEnchantments();
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
                    weapon.AddIntrinsicEnchantments();
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                weapon.AddIntrinsicEnchantments();
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

                if (Collections.StabbingSwords.Contains(sword.InitialParentTileIndex))
                {
                    sword.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            foreach (var sword in Game1.player.Items.OfType<MeleeWeapon>().Where(w =>
                         w.type.Value == MeleeWeapon.defenseSword &&
                         Collections.StabbingSwords.Contains(w.InitialParentTileIndex)))
            {
                sword.type.Value = MeleeWeapon.stabbingSword;
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
            foreach (var sword in Game1.player.Items.OfType<MeleeWeapon>().Where(w =>
                         w.type.Value == MeleeWeapon.stabbingSword))
            {
                sword.type.Value = MeleeWeapon.defenseSword;
            }
        }
    }

    /// <summary>Refreshes the stats of the all <see cref="MeleeWeapon"/>s in existence.</summary>
    internal static void RefreshAllWeapons()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon)
                {
                    return;
                }

                weapon.RefreshStats(true);
                MeleeWeapon_Stats.Invalidate(weapon);
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                weapon.RefreshStats(true);
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
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}
