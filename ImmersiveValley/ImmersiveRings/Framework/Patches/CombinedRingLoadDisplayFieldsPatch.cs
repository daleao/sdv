namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingLoadDisplayFieldsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingLoadDisplayFieldsPatch()
    {
        Target = RequireMethod<CombinedRing>("loadDisplayFields");
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium description is always first, and gemstone descriptions are grouped together.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingsLoadDisplayFieldsPrefix(CombinedRing __instance, ref bool __result)
    {
        if (__instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return true; // don't run original logic

        if (Game1.objectInformation is null || __instance.indexInTileSheet is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        var data = Game1.objectInformation[__instance.indexInTileSheet.Value].Split('/');
        __instance.displayName = data[4];
        __instance.description = data[5];
        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}