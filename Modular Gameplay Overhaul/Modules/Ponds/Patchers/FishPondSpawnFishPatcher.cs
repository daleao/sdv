namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
    internal FishPondSpawnFishPatcher()
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
                case ItemIDs.Seaweed:
                    pond.Increment(DataKeys.SeaweedLivingHere);
                    break;
                case ItemIDs.GreenAlgae:
                    pond.Increment(DataKeys.GreenAlgaeLivingHere);
                    break;
                case ItemIDs.WhiteAlgae:
                    pond.Increment(DataKeys.WhiteAlgaeLivingHere);
                    break;
            }

            var total = pond.Read<int>(DataKeys.SeaweedLivingHere) +
                        pond.Read<int>(DataKeys.GreenAlgaeLivingHere) +
                        pond.Read<int>(DataKeys.WhiteAlgaeLivingHere);
            if (total != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write(DataKeys.SeaweedLivingHere, null);
            pond.Write(DataKeys.GreenAlgaeLivingHere, null);
            pond.Write(DataKeys.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                ItemIDs.Seaweed => DataKeys.SeaweedLivingHere,
                ItemIDs.GreenAlgae => DataKeys.GreenAlgaeLivingHere,
                ItemIDs.WhiteAlgae => DataKeys.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            pond.Write(field, pond.FishCount.ToString());
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
                familyCount = pond.Read<int>(DataKeys.FamilyLivingHere);
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
            var qualities = pond
                .Read(forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, @default)
                .ParseList<int>();
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : pond.FishCount - familyCount - 1))
            {
                ThrowHelper.ThrowInvalidDataException("Mismatch between FishQualities data and actual population.");
            }

            if (qualities.Sum() == 0)
            {
                qualities[0]++;
                pond.Write(forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, string.Join(',', qualities));
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

            if (ProfessionsModule.IsEnabled && fishlingQuality < SObject.bestQuality && Game1.random.NextDouble() < 0.5 &&
                (pond.GetOwner().HasProfession(Professions.Profession.Aquarist) ||
                 (ProfessionsModule.Config.LaxOwnershipRequirements &&
                  Game1.game1.DoesAnyPlayerHaveProfession(Professions.Profession.Aquarist, out _))))
            {
                fishlingQuality += fishlingQuality == SObject.highQuality ? 2 : 1;
            }

            qualities[fishlingQuality == SObject.bestQuality ? 3 : fishlingQuality]++;
            pond.Write(forFamily ? DataKeys.FamilyQualities : DataKeys.FishQualities, string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write(DataKeys.FishQualities, $"{pond.FishCount},0,0,0");
            pond.Write(DataKeys.FamilyQualities, null);
            pond.Write(DataKeys.FamilyLivingHere, null);
        }
    }

    #endregion handlers
}
