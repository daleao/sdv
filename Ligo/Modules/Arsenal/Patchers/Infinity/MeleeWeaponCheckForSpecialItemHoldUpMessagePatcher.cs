namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

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

    /// <summary>Add Holy Blade obtain message.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCheckForSpecialItemHoldUpPostfix(MeleeWeapon __instance, ref string? __result)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne ||
            __instance.InitialParentTileIndex is not Constants.DarkSwordIndex or Constants.HolyBladeIndex)
        {
            return;
        }

        switch (__instance.InitialParentTileIndex)
        {
            case Constants.DarkSwordIndex:
            {
                var darkSword = ModEntry.i18n.Get("darksword.name");
                __result = ModEntry.i18n.Get("darksword.holdupmessage", new { darkSword });
                break;
            }

            case Constants.HolyBladeIndex:
            {
                var darkSword = ModEntry.i18n.Get("darksword.name");
                var holyBlade = ModEntry.i18n.Get("holyblade.name");
                __result = ModEntry.i18n.Get("holyblade.holdupmessage", new { darkSword, holyBlade });
                break;
            }
        }
    }

    #endregion harmony patches
}
