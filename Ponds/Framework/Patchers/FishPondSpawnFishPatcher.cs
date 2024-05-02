namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
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
            var spawned = r.NextAlgae(pond.fishType.Value);
            switch (spawned)
            {
                case QualifiedObjectIds.Seaweed:
                    Data.Increment(pond, DataKeys.SeaweedLivingHere);
                    break;
                case QualifiedObjectIds.GreenAlgae:
                    Data.Increment(pond, DataKeys.GreenAlgaeLivingHere);
                    break;
                case QualifiedObjectIds.WhiteAlgae:
                    Data.Increment(pond, DataKeys.WhiteAlgaeLivingHere);
                    break;
            }

            var total = Data.ReadAs<int>(pond, DataKeys.SeaweedLivingHere) +
                        Data.ReadAs<int>(pond, DataKeys.GreenAlgaeLivingHere) +
                        Data.ReadAs<int>(pond, DataKeys.WhiteAlgaeLivingHere);
            if (total != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(pond, DataKeys.SeaweedLivingHere, null);
            Data.Write(pond, DataKeys.GreenAlgaeLivingHere, null);
            Data.Write(pond, DataKeys.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                QualifiedObjectIds.Seaweed => DataKeys.SeaweedLivingHere,
                QualifiedObjectIds.GreenAlgae => DataKeys.GreenAlgaeLivingHere,
                QualifiedObjectIds.WhiteAlgae => DataKeys.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            Data.Write(pond, field, pond.FishCount.ToString());
        }
    }

    private static void SpawnFish(FishPond pond, Random r)
    {
        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (pond.HasLegendaryFish())
            {
                familyCount = Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions");
                if (familyCount < 0 || familyCount > pond.FishCount)
                {
                    ThrowHelper.ThrowInvalidDataException(
                        "FamilyLivingHere data is negative or greater than actual population.");
                }

                if (familyCount > 0 &&
                    Game1.random.NextDouble() <
                    (double)familyCount /
                    (pond.FishCount - 1)) // fish pond count has already been incremented at this point, so we consider -1
                {
                    forFamily = true;
                }
            }

            var @default = forFamily
                ? $"{familyCount},0,0,0"
                : $"{pond.FishCount - familyCount - 1},0,0,0";
            var qualities = Data
                .Read(pond, forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, @default)
                .ParseList<int>();
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : pond.FishCount - familyCount - 1))
            {
                ThrowHelper.ThrowInvalidDataException("Mismatch between FishQualities data and actual population.");
            }

            if (qualities.Sum() == 0)
            {
                qualities[0]++;
                Data.Write(pond, forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, string.Join(',', qualities));
                return;
            }

            var roll = r.Next(forFamily ? familyCount : pond.FishCount - familyCount - 1);
            var fishlingQuality = roll < qualities[3]
                ? SObject.bestQuality
                : roll < qualities[3] + qualities[2]
                    ? SObject.highQuality
                    : roll < qualities[3] + qualities[2] + qualities[1]
                        ? SObject.medQuality
                        : SObject.lowQuality;

            if (ModHelper.ModRegistry.IsLoaded("DaLion.Professions") && fishlingQuality < SObject.bestQuality &&
                pond.GetOwner().professions.Contains(Farmer.pirate))
            {
                fishlingQuality += fishlingQuality == SObject.highQuality ? 2 : 1;
            }

            qualities[fishlingQuality == SObject.bestQuality ? 3 : fishlingQuality]++;
            Data.Write(pond, forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(pond, DataKeys.FishQualities, $"{pond.FishCount},0,0,0");
            Data.Write(pond, DataKeys.FamilyQualities, null);
            Data.Write(pond, DataKeys.FamilyLivingHere, null, "DaLion.Professions");
        }
    }

    #endregion handlers
}
