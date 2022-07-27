namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnLeaveLocationPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnLeaveLocationPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.onLeaveLocation));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnLeaveLocationPrefix(Ring __instance) =>
        !ModEntry.Config.TheOneIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;

    #endregion harmony patches
}