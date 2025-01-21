namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondUpdateMaximumOccupancyPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondUpdateMaximumOccupancyPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondUpdateMaximumOccupancyPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Fix for Professions compatibility.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Professions")]
    [UsedImplicitly]
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance, int __state)
    {
        var fishes = __instance.ParsePondFishes();
        if (__instance.FishCount == fishes.Count)
        {
            return;
        }

        fishes.SortDescending();
        fishes = fishes.Take(__instance.FishCount).ToList();
        Data.Write(__instance, DataKeys.PondFish, string.Join(';', fishes));
    }

    #endregion harmony patches
}
