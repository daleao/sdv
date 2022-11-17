namespace DaLion.Ligo.Modules.Arsenal.Weapons;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using StardewValley;
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

                if (ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(sword.Name))
                {
                    sword.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            foreach (var sword in Game1.player.Items.OfType<MeleeWeapon>().Where(w =>
                         w.type.Value == MeleeWeapon.defenseSword &&
                         ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(w.Name)))
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
                if (item is MeleeWeapon weapon)
                {
                    UpdateSingleWeapon(weapon);
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                UpdateSingleWeapon(weapon);
            }
        }
    }

    private static void UpdateSingleWeapon(MeleeWeapon weapon)
    {
        var data = ModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>("Data/weapons");
        if (!data.ContainsKey(weapon.InitialParentTileIndex))
        {
            return;
        }

        var split = data[weapon.InitialParentTileIndex].Split('/');
        weapon.minDamage.Value = Convert.ToInt32(split[2]);
        weapon.maxDamage.Value = Convert.ToInt32(split[3]);
        weapon.knockback.Value = (float)Convert.ToDouble(split[4], CultureInfo.InvariantCulture);
        weapon.speed.Value = Convert.ToInt32(split[5]);
        weapon.addedPrecision.Value = Convert.ToInt32(split[6]);
        weapon.addedDefense.Value = Convert.ToInt32(split[7]);
        weapon.type.Set(Convert.ToInt32(split[8]));
        weapon.addedAreaOfEffect.Value = Convert.ToInt32(split[11]);
        weapon.critChance.Value = (float)Convert.ToDouble(split[12], CultureInfo.InvariantCulture);
        weapon.critMultiplier.Value = (float)Convert.ToDouble(split[13], CultureInfo.InvariantCulture);
    }
}
