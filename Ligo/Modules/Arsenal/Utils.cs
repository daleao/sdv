namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    /// <summary>Converts the config-specified defensive swords into stabbing swords throughout the world.</summary>
    internal static void ConvertAllStabbySwords()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } sword)
                {
                    return;
                }

                if (Collections.StabbySwords.Contains(sword.InitialParentTileIndex))
                {
                    sword.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            foreach (var sword in Game1.player.Items.OfType<MeleeWeapon>().Where(w =>
                         w.type.Value == MeleeWeapon.defenseSword &&
                         Collections.StabbySwords.Contains(w.InitialParentTileIndex)))
            {
                sword.type.Value = MeleeWeapon.stabbingSword;
            }
        }
    }

    /// <summary>Reverts all stabbing sword back into vanilla defensive swords.</summary>
    internal static void RevertAllStabbySwords()
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
    internal static void UpdateAllWeapons()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon)
                {
                    return;
                }

                weapon.RefreshStats();
                MeleeWeapon_Stats.Invalidate(weapon);
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                weapon.RefreshStats();
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
        player.mailReceived.Add("holyBlade");
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}
