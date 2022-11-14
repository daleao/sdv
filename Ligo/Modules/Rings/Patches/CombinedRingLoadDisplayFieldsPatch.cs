namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingLoadDisplayFieldsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingLoadDisplayFieldsPatch"/> class.</summary>
    internal CombinedRingLoadDisplayFieldsPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>("loadDisplayFields");
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium description is always first, and gemstone descriptions are grouped together.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingsLoadDisplayFieldsPrefix(CombinedRing __instance, ref bool __result)
    {
        if (__instance.ParentSheetIndex != Globals.InfinityBandIndex)
        {
            return true; // don't run original logic
        }

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
