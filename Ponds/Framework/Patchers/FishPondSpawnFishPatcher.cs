namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.IO;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondSpawnFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondSpawnFishPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondSpawnFishPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.SpawnFish));
    }

    #region harmony patches

    /// <summary>Set the quality of newborn fishes.</summary>
    [HarmonyPostfix]
    private static void FishPondSpawnFishPostfix(FishPond __instance)
    {
        if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
            !__instance.hasSpawnedFish.Value)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        if (__instance.HasAlgae())
        {
            SpawnAlgae(__instance, r);
        }
        else
        {
            SpawnFish(__instance, r);
        }
    }

    #endregion harmony patches

    #region handlers

    private static void SpawnAlgae(FishPond pond, Random r)
    {
        try
        {
            var spawned = new PondFish(r.NextAlgae(pond.fishType.Value), SObject.lowQuality);
            Data.Append(pond, DataKeys.PondFish, spawned.ToString(), ';');
            if (Data.Read(pond, DataKeys.PondFish).Split(';').Length != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.ResetPondFishData();
        }
    }

    private static void SpawnFish(FishPond pond, Random r)
    {
        try
        {
            var spawned = pond.ParsePondFishes().Choose() ??
                          new PondFish($"(O){pond.fishType.Value}", SObject.lowQuality);
            if (ModHelper.ModRegistry.IsLoaded("DaLion.Professions") && spawned.Quality < SObject.bestQuality &&
                pond.GetOwner().professions.Contains(Farmer.pirate))
            {
                spawned = spawned with { Quality = spawned.Quality + (spawned.Quality == SObject.highQuality ? 2 : 1) };
            }

            Data.Append(pond, DataKeys.PondFish, spawned.ToString(), ';');
            if (Data.Read(pond, DataKeys.PondFish).Split(';').Length != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.ResetPondFishData();
        }
    }

    #endregion handlers
}
