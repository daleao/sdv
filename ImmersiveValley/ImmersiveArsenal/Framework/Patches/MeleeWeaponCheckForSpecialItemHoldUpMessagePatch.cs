namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCheckForSpecialItemHoldUpMessagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCheckForSpecialItemHoldUpMessagePatch"/> class.</summary>
    internal MeleeWeaponCheckForSpecialItemHoldUpMessagePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add Dark Sword mod data.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCheckForSpecialItemHoldUpPostfix(MeleeWeapon __instance, ref string? __result)
    {
        if (ModEntry.Config.InfinityPlusOneWeapons && __instance.InitialParentTileIndex == Constants.HolyBladeIndex)
        {
            __result = ModEntry.i18n.Get("holyblade.holdupmessage");
        }
    }

    #endregion harmony patches
}
