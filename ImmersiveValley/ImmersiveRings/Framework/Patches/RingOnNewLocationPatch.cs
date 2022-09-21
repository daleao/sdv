namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnNewLocationPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingOnNewLocationPatch"/> class.</summary>
    internal RingOnNewLocationPatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onNewLocation));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnNewLocationPrefix(Ring __instance)
    {
        return !ModEntry.Config.TheOneIridiumBand ||
               __instance.indexInTileSheet.Value != Constants.IridiumBandIndex;
    }

    #endregion harmony patches
}
