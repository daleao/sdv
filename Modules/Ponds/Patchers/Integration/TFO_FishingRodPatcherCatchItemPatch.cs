namespace DaLion.Overhaul.Modules.Ponds.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[ModRequirement("TehPers.FishingOverhaul", "Teh's Fishing Overhaul")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class FishingRodPatcherCatchItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPatcherCatchItemPatcher"/> class.</summary>
    internal FishingRodPatcherCatchItemPatcher()
    {
        this.Target = "TehPers.FishingOverhaul.Services.Setup.FishingRodPatcher"
            .ToType()
            .RequireMethod("CatchItem");
    }

    #region harmony patches

    /// <summary>Corrects Fish Pond data after pulling fish from pond.</summary>
    [HarmonyPrefix]
    private static void FishingRodPatcherCatchItemPrefix(object info)
    {
        try
        {
            if (!info.GetType().Name.Contains("FishCatch"))
            {
                return;
            }

            var fromFishPond = Reflector
                .GetUnboundPropertyGetter<object, bool>(info, "FromFishPond")
                .Invoke(info);
            if (!fromFishPond)
            {
                return;
            }

            var fishingInfo = Reflector
                .GetUnboundPropertyGetter<object, object>(info, "FishingInfo")
                .Invoke(info);
            var (x, y) = Reflector
                .GetUnboundPropertyGetter<object, Vector2>(fishingInfo, "BobberPosition")
                .Invoke(fishingInfo);
            var pond = Game1.getFarm().buildings
                .OfType<FishPond>()
                .FirstOrDefault(p =>
                    x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                    y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null)
            {
                return;
            }

            var setFishItem = Reflector
                .GetUnboundPropertySetter<object, Item>(info, "FishItem");
            var setFishQuality = Reflector
                .GetUnboundPropertySetter<object, int>(info, "FishQuality");
            if (pond.HasAlgae())
            {
                PullAlgae(pond, setFishItem, setFishQuality, info);
            }

            PullFish(pond, setFishItem, setFishQuality, info);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches

    #region handlers

    private static void PullAlgae(FishPond pond, Action<object, Item> setFishItem, Action<object, int> setFishQuality, object fishCatch)
    {
        setFishQuality(fishCatch, SObject.lowQuality);
        try
        {
            var seaweedCount = pond.Read<int>(DataKeys.SeaweedLivingHere);
            var greenAlgaeCount = pond.Read<int>(DataKeys.GreenAlgaeLivingHere);
            var whiteAlgaeCount = pond.Read<int>(DataKeys.WhiteAlgaeLivingHere);

            var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
            var index = -1;
            if (roll < seaweedCount)
            {
                index = ObjectIds.Seaweed;
                pond.Write(DataKeys.SeaweedLivingHere, (--seaweedCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount)
            {
                index = ObjectIds.GreenAlgae;
                pond.Write(DataKeys.GreenAlgaeLivingHere, (--greenAlgaeCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
            {
                index = ObjectIds.WhiteAlgae;
                pond.Write(DataKeys.WhiteAlgaeLivingHere, (--whiteAlgaeCount).ToString());
            }

            if (index < 0)
            {
                // I don't expect this to ever throw
                throw new Exception();
            }

            setFishItem(fishCatch, new SObject(index, 1));
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
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            pond.Write(DataKeys.SeaweedLivingHere, null);
            pond.Write(DataKeys.GreenAlgaeLivingHere, null);
            pond.Write(DataKeys.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                ObjectIds.Seaweed => DataKeys.SeaweedLivingHere,
                ObjectIds.GreenAlgae => DataKeys.GreenAlgaeLivingHere,
                ObjectIds.WhiteAlgae => DataKeys.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            pond.Write(field, pond.FishCount.ToString());
        }
    }

    private static void PullFish(FishPond pond, Action<object, Item> setFishItem, Action<object, int> setFishQuality, object info)
    {
        try
        {
            var fishQualities = pond.Read(
                    DataKeys.FishQualities,
                    $"{pond.FishCount - pond.Read<int>(DataKeys.FamilyLivingHere) + 1},0,0,0")
                .ParseList<int>(); // already reduced at this point, so consider + 1
            if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > pond.FishCount + 1))
            {
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
            }

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.HasLegendaryFish())
            {
                HandleLegendary(pond, setFishItem, setFishQuality, info, fishQualities, lowestFish);
            }
            else
            {
                if (fishQualities.Sum() != pond.FishCount + 1)
                {
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                }

                setFishItem(info, new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                fishQualities[lowestFish]--;
                pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            pond.Write(DataKeys.FishQualities, $"{pond.FishCount},0,0,0");
            pond.Write(DataKeys.FamilyQualities, null);
            pond.Write(DataKeys.FamilyLivingHere, null);
        }
    }

    private static void HandleLegendary(
        FishPond pond, Action<object, Item> setFishItem, Action<object, int> setFishQuality, object info, List<int> fishQualities, int lowestFish)
    {
        var familyCount = pond.Read<int>(DataKeys.FamilyLivingHere);
        if (fishQualities.Sum() + familyCount != pond.FishCount + 1)
        {
            ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
        }

        if (familyCount > 0)
        {
            var familyQualities =
                pond.Read(DataKeys.FamilyQualities, $"{familyCount},0,0,0").ParseList<int>();
            if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
            {
                ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
            }

            var lowestFamily = familyQualities.FindIndex(i => i > 0);
            if (lowestFamily < lowestFish)
            {
                var whichFish = Maps.ExtendedFamilyPairs[pond.fishType.Value];
                setFishItem(
                    info,
                    new SObject(whichFish, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                setFishQuality(info, lowestFamily == 3 ? 4 : lowestFamily);
                familyQualities[lowestFamily]--;
                pond.Write(DataKeys.FamilyQualities, string.Join(",", familyQualities));
                pond.Increment(DataKeys.FamilyLivingHere, -1);
            }
            else
            {
                setFishItem(
                    info,
                    new SObject(pond.fishType.Value, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                fishQualities[lowestFish]--;
                pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        else
        {
            setFishItem(
                info,
                new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
            setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
            fishQualities[lowestFish]--;
            pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
        }
    }

    #endregion handlers
}
