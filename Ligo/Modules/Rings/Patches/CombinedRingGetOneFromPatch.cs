namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using HarmonyLib;
using Netcode;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingGetOneFromPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingGetOneFromPatch"/> class.</summary>
    internal CombinedRingGetOneFromPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing._GetOneFrom));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Changes combined ring to Infinity Band when getting one.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingGetOneFromPrefix(CombinedRing __instance, Item source)
    {
        if (source.ParentSheetIndex != Globals.InfinityBandIndex)
        {
            return true; // run original logic
        }

        __instance.ParentSheetIndex = Globals.InfinityBandIndex;
        ModEntry.ModHelper.Reflection.GetField<NetInt>(__instance, nameof(Ring.indexInTileSheet)).GetValue()
            .Set(Globals.InfinityBandIndex);
        return true; // run original logic
    }

    #endregion harmony patches
}
