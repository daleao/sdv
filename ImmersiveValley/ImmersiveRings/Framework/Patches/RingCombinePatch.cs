namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Netcode;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCombinePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingCombinePatch"/> class.</summary>
    internal RingCombinePatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.Combine));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Changes combined ring to Infinity Band when combining.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCombinePrefix(Ring __instance, ref Ring __result, Ring ring)
    {
        if (!ModEntry.Config.TheOneIridiumBand || __instance.ParentSheetIndex != ModEntry.InfinityBandIndex)
        {
            return true; // run original logic
        }

        try
        {
            var toCombine = new List<Ring>();
            if (__instance is CombinedRing combined)
            {
                if (combined.combinedRings.Count >= 4)
                {
                    ThrowHelper.ThrowInvalidOperationException("Unexpected number of combined rings.");
                }

                toCombine.AddRange(combined.combinedRings);
            }

            toCombine.Add(ring);
            var combinedRing = new CombinedRing(880);
            combinedRing.combinedRings.AddRange(toCombine);
            combinedRing.ParentSheetIndex = ModEntry.InfinityBandIndex;
            ModEntry.ModHelper.Reflection.GetField<NetInt>(combinedRing, nameof(Ring.indexInTileSheet)).GetValue()
                .Set(ModEntry.InfinityBandIndex);
            combinedRing.UpdateDescription();
            __result = combinedRing;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
