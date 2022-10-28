namespace DaLion.Redux.Arsenal.Weapons;

#region using directives

using System.Linq;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    /// <summary>Converts the config-specified defensive swords into stabbing swords throughout the world.</summary>
    internal static void ConvertStabbySwords()
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
    internal static void RevertStabbySwords()
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
}
