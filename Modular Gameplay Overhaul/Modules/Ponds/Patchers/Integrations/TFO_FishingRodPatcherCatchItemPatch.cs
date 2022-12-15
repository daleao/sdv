namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[RequiresMod("TehPers.FishingOverhaul")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
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
        FishPond? pond = null;
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
            pond = Game1.getFarm().buildings
                .OfType<FishPond>()
                .FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null)
            {
                return;
            }

            var fishQualities = pond.Read(
                    DataFields.FishQualities,
                    $"{pond.FishCount - pond.Read<int>(DataFields.FamilyLivingHere) + 1},0,0,0")
                .ParseList<int>(); // already reduced at this point, so consider + 1
            if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > pond.FishCount + 1))
            {
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
            }

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            var setFishItem = Reflector
                .GetUnboundPropertySetter<object, object>(info, "FishItem");
            var setFishQuality = Reflector
                .GetUnboundPropertySetter<object, object>(info, "FishQuality");
            if (pond.HasLegendaryFish())
            {
                var familyCount = pond.Read<int>(DataFields.FamilyLivingHere);
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1)
                {
                    ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
                }

                if (familyCount > 0)
                {
                    var familyQualities = pond.Read(DataFields.FamilyQualities, $"{familyCount},0,0,0").ParseList<int>();
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                    {
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                    }

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish)
                    {
                        var whichFish = Collections.ExtendedFamilyPairs[pond.fishType.Value];
                        setFishItem(
                            info,
                            new SObject(whichFish, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        setFishQuality(info, lowestFamily == 3 ? 4 : lowestFamily);
                        familyQualities[lowestFamily]--;
                        pond.Write(DataFields.FamilyQualities, string.Join(",", familyQualities));
                        pond.Increment(DataFields.FamilyLivingHere, -1);
                    }
                    else
                    {
                        setFishItem(
                            info,
                            new SObject(pond.fishType.Value, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                        fishQualities[lowestFish]--;
                        pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                    }
                }
                else
                {
                    setFishItem(
                        info,
                        new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                    setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                    fishQualities[lowestFish]--;
                    pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                }
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
                pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex) when (pond is not null)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write(DataFields.FishQualities, $"{pond.FishCount},0,0,0");
            pond.Write(DataFields.FamilyQualities, null);
            pond.Write(DataFields.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
