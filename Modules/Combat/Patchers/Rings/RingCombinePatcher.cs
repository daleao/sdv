namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCombinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingCombinePatcher"/> class.</summary>
    internal RingCombinePatcher()
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
        if (!CombatModule.Config.RingsEnchantments.EnableInfinityBand || !JsonAssetsIntegration.InfinityBandIndex.HasValue ||
            __instance.ParentSheetIndex != JsonAssetsIntegration.InfinityBandIndex)
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
            var combinedRing = new CombinedRing(ObjectIds.CombinedRing);
            combinedRing.combinedRings.AddRange(toCombine);
            combinedRing.ParentSheetIndex = JsonAssetsIntegration.InfinityBandIndex.Value;
            ModHelper.Reflection.GetField<NetInt>(combinedRing, nameof(Ring.indexInTileSheet)).GetValue()
                .Set(JsonAssetsIntegration.InfinityBandIndex.Value);
            combinedRing.UpdateDescription();
            combinedRing.Get_Chord()?.PlayCues();
            Game1.player.WriteIfNotExists(DataKeys.HasMadeInfinityBand, "true");
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
