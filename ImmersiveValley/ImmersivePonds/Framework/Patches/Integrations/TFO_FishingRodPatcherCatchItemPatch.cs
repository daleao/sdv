namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Attributes;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using LinqFasterer;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using System;
using System.IO;
using System.Linq;

#endregion using directives

[UsedImplicitly, RequiresMod("TehPers.FishingOverhau")]
internal sealed class FishingRodPatcherCatchItemPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, bool>? _GetFromFishPond;
    private static Func<object, object>? _GetFishingInfo;
    private static Func<object, Vector2>? _GetBobberPosition;
    private static Action<object, object>? _SetFishItem;
    private static Action<object, object>? _SetFishQuality;

    /// <summary>Construct an instance.</summary>
    internal FishingRodPatcherCatchItemPatch()
    {
        Target = "TehPers.FishingOverhaul.Services.Setup.FishingRodPatcher".ToType().RequireMethod("CatchItem");
    }

    #region harmony patches

    /// <summary>Corrects Fish Pond data after pulling fish from pond.</summary>
    [HarmonyPrefix]
    private static void FishingRodPatcherCatchItemPrefix(object info)
    {
        FishPond? pond = null;
        try
        {
            if (!info.GetType().Name.Contains("FishCatch")) return;

            _GetFromFishPond ??= info.GetType().RequirePropertyGetter("FromFishPond")
                .CompileUnboundDelegate<Func<object, bool>>();
            var fromFishPond = _GetFromFishPond(info);
            if (!fromFishPond) return;

            _GetFishingInfo ??= info.GetType().RequirePropertyGetter("FishingInfo")
                .CompileUnboundDelegate<Func<object, object>>();
            var fishingInfo = _GetFishingInfo(info);
            _GetBobberPosition ??= fishingInfo.GetType().RequirePropertyGetter("BobberPosition")
                .CompileUnboundDelegate<Func<object, Vector2>>();
            var (x, y) = _GetBobberPosition(fishingInfo);
            pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null) return;

            var fishQualities = pond.Read("FishQualities",
                $"{pond.FishCount - pond.Read<int>("FamilyLivingHere") + 1},0,0,0").ParseList<int>(); // already reduced at this point, so consider + 1
            if (fishQualities.Count != 4 || fishQualities.AnyF(q => 0 > q || q > pond.FishCount + 1))
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            _SetFishItem ??= info.GetType().RequirePropertySetter("FishItem").CompileUnboundDelegate<Action<object, object>>();
            _SetFishQuality ??= info.GetType().RequirePropertySetter("FishQuality").CompileUnboundDelegate<Action<object, object>>();
            if (pond.HasLegendaryFish())
            {
                var familyCount = pond.Read<int>("FamilyLivingHere");
                if (fishQualities.SumF() + familyCount != pond.FishCount + 1)
                    ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities = pond.Read("FamilyQualities", $"{familyCount},0,0,0").ParseList<int>();
                    if (familyQualities.Count != 4 || familyQualities.SumF() != familyCount)
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish)
                    {
                        var whichFish = Utils.ExtendedFamilyPairs[pond.fishType.Value];
                        _SetFishItem(info, new SObject(whichFish, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        _SetFishQuality(info, lowestFamily == 3 ? 4 : lowestFamily);
                        --familyQualities[lowestFamily];
                        pond.Write("FamilyQualities", string.Join(",", familyQualities));
                        pond.Increment("FamilyLivingHere", -1);
                    }
                    else
                    {
                        _SetFishItem(info,
                            new SObject(pond.fishType.Value, 1, quality: lowestFamily == 3 ? 4 : lowestFamily));
                        _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                        --fishQualities[lowestFish];
                        pond.Write("FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    _SetFishItem(info,
                        new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                    _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                    --fishQualities[lowestFish];
                    pond.Write("FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                if (fishQualities.SumF() != pond.FishCount + 1)
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");

                _SetFishItem(info, new SObject(pond.fishType.Value, 1, quality: lowestFish == 3 ? 4 : lowestFish));
                _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                --fishQualities[lowestFish];
                pond.Write("FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex) when (pond is not null)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write("FishQualities", $"{pond.FishCount},0,0,0");
            pond.Write("FamilyQualities", null);
            pond.Write("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}