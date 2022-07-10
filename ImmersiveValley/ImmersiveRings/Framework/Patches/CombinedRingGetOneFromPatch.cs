namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingGetOneFromPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingGetOneFromPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing._GetOneFrom));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Changes combined ring to iridium band when getting one.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingGetOneFromPrefix(CombinedRing __instance, Item source)
    {
        if (source.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return true; // run original logic

        __instance.ParentSheetIndex = Constants.IRIDIUM_BAND_INDEX_I;
        ModEntry.ModHelper.Reflection.GetField<NetInt>(__instance, nameof(Ring.indexInTileSheet)).GetValue()
            .Set(Constants.IRIDIUM_BAND_INDEX_I);
        return true; // run original logic
    }

    #endregion harmony patches
}