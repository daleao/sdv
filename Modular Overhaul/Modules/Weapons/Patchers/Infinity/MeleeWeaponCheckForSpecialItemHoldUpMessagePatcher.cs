namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher"/> class.</summary>
    internal MeleeWeaponCheckForSpecialItemHoldUpMessagePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add obtain legendary weapon messages.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCheckForSpecialItemHoldUpPostfix(MeleeWeapon __instance, ref string? __result)
    {
        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return true; // run original logic
        }

        try
        {
            if (__instance.isGalaxyWeapon())
            {
                var count = Game1.player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>().Count;
                __result = count == 1
                    ? I18n.Get("fromcsfiles.MeleeWeapon.cs.14122", new { galaxyWeapon = __instance.DisplayName })
                    : (string?)null;
                return false; // don't run original logic
            }

            switch (__instance.InitialParentTileIndex)
            {
                case ItemIDs.DarkSword:
                {
                    var darkSword = I18n.Get("weapons.darksword.name");
                    __result = I18n.Get("weapons.darksword.holdupmessage", new { darkSword });
                    break;
                }

                case ItemIDs.HolyBlade:
                {
                    var darkSword = I18n.Get("weapons.darksword.name");
                    var holyBlade = I18n.Get("weapons.holyblade.name");
                    __result = I18n.Get("weapons.holyblade.holdupmessage", new { darkSword, holyBlade });
                    break;
                }
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
