namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnNewLocationPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnNewLocationPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.onNewLocation));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnNewLocationPrefix(Ring __instance, Farmer who)
    {
        return !ModEntry.Config.ForgeableIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;
    }

    #endregion harmony patches
}