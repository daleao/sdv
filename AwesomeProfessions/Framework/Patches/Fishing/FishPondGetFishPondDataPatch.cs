namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

using Objects = Utility.Objects;

#endregion using directives

[UsedImplicitly]
internal class FishPondGetFishPondDataPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondGetFishPondDataPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.GetFishPondData));
    }

    #region harmony patches

    /// <summary>Patch to get fish pond data for legendary fish.</summary>
    [HarmonyPostfix]
    private static void FishPondGetFishPondDataPostfix(ref FishPond __instance, ref FishPondData __result,
        ref FishPondData ____fishPondData)
    {
        if (__instance.fishType.Value <= 0) return;

        var fishName = __instance.GetFishObject().Name;
        if (!Objects.LegendaryFishNames.Contains(fishName)) return;

        if (____fishPondData is not null)
        {
            ____fishPondData.SpawnTime = fishName.Contains("Legend") ? 10 : 7;
            return;
        }

        ____fishPondData = new()
        {
            PopulationGates = null,
            ProducedItems = new()
            {
                new()
                {
                    Chance = 0.9f,
                    ItemID = 812, // roe
                    MinQuantity = 1,
                    MaxQuantity = 1
                }
            },
            RequiredTags = new(),
            SpawnTime = fishName.Contains("Legend") ? 10 : 7
        };
        __result = ____fishPondData;
    }

    #endregion harmony patches
}