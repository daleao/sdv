namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

using Common.Extensions;
using Common.Extensions.Reflection;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

internal static class TehsFishingOverhaulPatches
{
    private static MethodInfo _GetFishingInfo, _GetFromFishPond, _GetBobberPosition, _SetFishItem, _SetFishQuality;

    /// <summary>Apply integration patches.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    internal static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: "TehPers.FishingOverhaul.Services.Setup.FishingRodPatcher".ToType().RequireMethod("CatchItem"),
            prefix: new(typeof(TehsFishingOverhaulPatches).RequireMethod(nameof(SetupCatchItemPrefix)))
        );
    }

    #region harmony patches

    /// <summary>Corrects Fish Pond data after pulling fish from pond.</summary>
    private static void SetupCatchItemPrefix(object info)
    {
        FishPond pond = null;
        try
        {
            if (!info.GetType().Name.Contains("FishCatch")) return;

            _GetFromFishPond ??= info.GetType().RequirePropertyGetter("FromFishPond");
            var fromFishPond = (bool) _GetFromFishPond.Invoke(info, null)!;
            if (!fromFishPond) return;

            _GetFishingInfo ??= info.GetType().RequirePropertyGetter("FishingInfo");
            var fishingInfo = _GetFishingInfo.Invoke(info, null);
            if (fishingInfo is null) return;

            _GetBobberPosition ??= fishingInfo.GetType().RequirePropertyGetter("BobberPosition");
            var (x, y) = (Vector2) _GetBobberPosition.Invoke(fishingInfo, null)!;
            pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null) return;

            var fishQualities =
                pond.ReadData("FishQualities", $"{pond.FishCount - pond.ReadDataAs<int>("FamilyLivingHere") + 1},0,0,0") // already reduced at this point, so consider + 1
                    .ParseList<int>()!;
            if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1))
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            _SetFishQuality ??= info.GetType().RequirePropertySetter("FishQuality");
            _SetFishItem ??= info.GetType().RequirePropertySetter("FishItem");
            if (pond.IsLegendaryPond())
            {
                var familyCount = pond.ReadDataAs<int>("FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1)
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities = pond.ReadData("FamilyQualities", $"{familyCount},0,0,0").ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish)
                    {
                        var whichFish = Framework.Utility.ExtendedFamilyPairs[pond.fishType.Value];
                        _SetFishItem.Invoke(info,
                            new[]
                            {
                                (object) new SObject(whichFish, 1) {Quality = lowestFamily == 3 ? 4 : lowestFamily}
                            });
                        _SetFishQuality.Invoke(info, new[] {(object) (lowestFamily == 3 ? 4 : lowestFamily)});
                        --familyQualities[lowestFamily];
                        pond.WriteData("FamilyQualities", string.Join(",", familyQualities));
                        pond.IncrementData("FamilyLivingHere", -1);
                    }
                    else
                    {
                        _SetFishItem.Invoke(info,
                            new[]
                            {
                                (object) new SObject(pond.fishType.Value, 1) {Quality = lowestFamily == 3 ? 4 : lowestFamily}
                            });
                        _SetFishQuality.Invoke(info, new[] {(object) (lowestFish == 3 ? 4 : lowestFish)});
                        --fishQualities[lowestFish];
                        pond.WriteData("FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    _SetFishItem.Invoke(info,
                        new[]
                        {
                            (object) new SObject(pond.fishType.Value, 1) {Quality = lowestFish == 3 ? 4 : lowestFish}
                        });
                    _SetFishQuality.Invoke(info, new[] {(object) (lowestFish == 3 ? 4 : lowestFish)});
                    --fishQualities[lowestFish];
                    pond.WriteData("FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                if (fishQualities.Sum() != pond.FishCount + 1)
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                _SetFishItem.Invoke(info,
                    new[]
                    {
                        (object) new SObject(pond.fishType.Value, 1) {Quality = lowestFish == 3 ? 4 : lowestFish}
                    });
                _SetFishQuality.Invoke(info, new[] {(object) (lowestFish == 3 ? 4 : lowestFish)});
                --fishQualities[lowestFish];
                pond.WriteData("FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex) when (pond is not null)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.WriteData("FishQualities", $"{pond.FishCount},0,0,0");
            pond.WriteData("FamilyQualities", null);
            pond.WriteData("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}