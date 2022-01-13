using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondUpdateMaximumOccupancyPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
    [HarmonyPostfix]
    private static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance,
        FishPondData ____fishPondData)
    {
        if (__instance is null || ____fishPondData is null) return;

        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (owner.HasProfession("Aquarist") && (____fishPondData.PopulationGates is null ||
                                                __instance.lastUnlockedPopulationGate.Value >=
                                                ____fishPondData.PopulationGates.Keys.Max()))
            __instance.maxOccupants.Set(12);
    }

    #endregion harmony patches
}