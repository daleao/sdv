namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCombinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingCombinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingCombinePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
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
        if (__instance.ItemId != InfinityBandId)
        {
            return true; // run original logic
        }

        try
        {
            List<Ring> toCombine = [];
            if (__instance is CombinedRing combined)
            {
                if (combined.combinedRings.Count >= 4)
                {
                    ThrowHelper.ThrowInvalidOperationException("Unexpected number of combined rings.");
                }

                toCombine.AddRange(combined.combinedRings);
            }

            toCombine.Add(ring);
            var combinedRing = new CombinedRing { ItemId = InfinityBandId };
            combinedRing.combinedRings.AddRange(toCombine);
            if (Config.AudibleGemstones)
            {
                combinedRing.Get_Chord()?.PlayCues();
            }

            Data.WriteIfNotExists(Game1.player, DataKeys.HasMadeInfinityBand, "true");
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
