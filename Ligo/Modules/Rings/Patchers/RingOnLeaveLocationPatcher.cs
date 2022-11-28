namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnLeaveLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnLeaveLocationPatcher"/> class.</summary>
    internal RingOnLeaveLocationPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onLeaveLocation));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnLeaveLocationPrefix(Ring __instance)
    {
        return !ModEntry.Config.Rings.TheOneInfinityBand ||
               __instance.indexInTileSheet.Value != Constants.IridiumBandIndex;
    }

    #endregion harmony patches
}
