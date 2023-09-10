namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

using DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCanCombinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingCanCombinePatcher"/> class.</summary>
    internal RingCanCombinePatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.CanCombine));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Allows feeding up to four gemstone rings into an Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCanCombinePrefix(Ring __instance, ref bool __result, Ring ring)
    {
        if (!CombatModule.Config.EnableInfinityBand || !Globals.InfinityBandIndex.HasValue)
        {
            return true; // run original logic
        }

        if (__instance.ParentSheetIndex == ItemIDs.IridiumBand ||
            ring.ParentSheetIndex == ItemIDs.IridiumBand ||
            ring.ParentSheetIndex == Globals.InfinityBandIndex.Value)
        {
            return false; // don't run original logic
        }

        if (__instance.ParentSheetIndex != Globals.InfinityBandIndex.Value)
        {
            return true; // run original logic
        }

        __result = ring.IsGemRing() &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
