namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnNewLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnNewLocationPatcher"/> class.</summary>
    internal RingOnNewLocationPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onNewLocation));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnNewLocationPrefix(Ring __instance)
    {
        return !ModEntry.Config.Rings.TheOneInfinityBand ||
               __instance.indexInTileSheet.Value != Constants.IridiumBandIndex;
    }

    #endregion harmony patches
}
