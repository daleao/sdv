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
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondAddFishToPondPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Add to PondFish data.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, FishPondData ____fishPondData, SObject fish)
    {
        var added = new PondFish(fish.ItemId, fish.Quality);
        Data.Append(__instance, DataKeys.PondFish, added.ToString(), ';');
        if (Data.Read(__instance, DataKeys.PondFish).Split(';').Length == __instance.FishCount)
        {
            return;
        }

        Log.W($"Mismatch between fish population data and actual population:" +
              $"\n\t- Population: {__instance.FishCount}\n\t- Data: {Data.Read(__instance, DataKeys.PondFish)}");
        Log.W("The data will be reset.");
        __instance.ResetPondFishData();
    }

    #endregion harmony patches
}
