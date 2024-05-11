namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondAddFishToPondPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, FishPondData ____fishPondData, SObject fish)
    {

        var added = new PondFish(fish.ItemId, fish.Quality);
        Data.Append(__instance, DataKeys.PondFish, added.ToString(), ';');
        if (fish.ItemId is "160" or "899")
        {
           __instance.SetAnglerSpawnTime(____fishPondData);
        }

        if (Data.Read(__instance, DataKeys.PondFish).Split(';').Length == __instance.FishCount)
        {
            return;
        }

        Log.E("Mismatch between fish population data and actual population.");
        __instance.ResetPondFishData();
    }

    #endregion harmony patches
}
