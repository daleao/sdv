namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnLeaveLocationPatch : BasePatch
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
    private static bool RingOnLeaveLocationPrefix(Ring __instance, Farmer who)
    {
        return !ModEntry.Config.ForgeableIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;
    }

    #endregion harmony patches
}