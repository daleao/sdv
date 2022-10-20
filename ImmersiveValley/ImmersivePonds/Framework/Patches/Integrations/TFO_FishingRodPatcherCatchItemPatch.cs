namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Ponds.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("TehPers.FishingOverhaul")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class FishingRodPatcherCatchItemPatch : HarmonyPatch
{
    private static Func<object, Vector2>? _getBobberPosition;
    private static Func<object, object>? _getFishingInfo;
    private static Func<object, bool>? _getFromFishPond;
    private static Action<object, object>? _setFishItem;
    private static Action<object, object>? _setFishQuality;

    /// <summary>Initializes a new instance of the <see cref="FishingRodPatcherCatchItemPatch"/> class.</summary>
    internal FishingRodPatcherCatchItemPatch()
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

            _getFromFishPond ??= info
                .GetType()
                .RequirePropertyGetter("FromFishPond")
                .CompileUnboundDelegate<Func<object, bool>>();
            var fromFishPond = _getFromFishPond(info);
            if (!fromFishPond)
            {
                return;
            }

            _getFishingInfo ??= info
                .GetType()
                .RequirePropertyGetter("FishingInfo")
                .CompileUnboundDelegate<Func<object, object>>();
            var fishingInfo = _getFishingInfo(info);
            _getBobberPosition ??= fishingInfo
                .GetType()
                .RequirePropertyGetter("BobberPosition")
                .CompileUnboundDelegate<Func<object, Vector2>>();
            var (x, y) = _getBobberPosition(fishingInfo);
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
            _setFishItem ??= info
                .GetType()
                .RequirePropertySetter("FishItem")
                .CompileUnboundDelegate<Action<object, object>>();
            _setFishQuality ??= info
                .GetType()
                .RequirePropertySetter("FishQuality")
                .CompileUnboundDelegate<Action<object, object>>();
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
                        var whichFish = Utils.ExtendedFamilyPairs[pond.fishType.Value];
                        _setFishItem(
                            info,
                            new SObject(whichFish, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        _setFishQuality(info, lowestFamily == 3 ? 4 : lowestFamily);
                        --familyQualities[lowestFamily];
                        pond.Write(DataFields.FamilyQualities, string.Join(",", familyQualities));
                        pond.Increment(DataFields.FamilyLivingHere, -1);
                    }
                    else
                    {
                        _setFishItem(
                            info,
                            new SObject(pond.fishType.Value, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        _setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                        --fishQualities[lowestFish];
                        pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                    }
                }
                else
                {
                    _setFishItem(
                        info,
                        new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                    _setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                    --fishQualities[lowestFish];
                    pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                }
            }
            else
            {
                if (fishQualities.Sum() != pond.FishCount + 1)
                {
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                }

                _setFishItem(info, new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                _setFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                --fishQualities[lowestFish];
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
