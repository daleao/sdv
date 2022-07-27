namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCheckForSpecialItemHoldUpPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponCheckForSpecialItemHoldUpPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add Dark Sword mod data</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCheckForSpecialItemHoldUpPostfix(MeleeWeapon __instance, ref string? __result)
    {
        if (ModEntry.Config.InfinityPlusOneWeapons && __instance.InitialParentTileIndex == Constants.HOLY_BLADE_INDEX_I)
            __result = ModEntry.i18n.Get("holyblade.holdupmessage");
    }

    #endregion harmony patches
}