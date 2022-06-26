namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
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
        try
        {
            Target = "TehPers.FishingOverhaul.Services.Setup.FishingRodPatcher".ToType().RequireMethod("CatchItem");
        }
        catch
        {
            // ignored
        }
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

            var fishQualities = ModDataIO.ReadData(pond, "FishQualities",
                $"{pond.FishCount - ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere") + 1},0,0,0").ParseList<int>()!; // already reduced at this point, so consider + 1
            if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1))
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            _SetFishItem ??= info.GetType().RequirePropertySetter("FishItem").CompileUnboundDelegate<Action<object, object>>();
            _SetFishQuality ??= info.GetType().RequirePropertySetter("FishQuality").CompileUnboundDelegate<Action<object, object>>();
            if (pond.IsLegendaryPond())
            {
                var familyCount = ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1)
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities = ModDataIO.ReadData(pond, "FamilyQualities", $"{familyCount},0,0,0").ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish)
                    {
                        var whichFish = Utils.ExtendedFamilyPairs[pond.fishType.Value];
                        _SetFishItem(info, new SObject(whichFish, 1) {Quality = lowestFamily == 3 ? 4 : lowestFamily});
                        _SetFishQuality(info, lowestFamily == 3 ? 4 : lowestFamily);
                        --familyQualities[lowestFamily];
                        ModDataIO.WriteData(pond, "FamilyQualities", string.Join(",", familyQualities));
                        ModDataIO.IncrementData(pond, "FamilyLivingHere", -1);
                    }
                    else
                    {
                        _SetFishItem(info,
                            new SObject(pond.fishType.Value, 1) {Quality = lowestFamily == 3 ? 4 : lowestFamily});
                        _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                        --fishQualities[lowestFish];
                        ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    _SetFishItem(info,
                        new SObject(pond.fishType.Value, 1) {Quality = lowestFish == 3 ? 4 : lowestFish});
                    _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                    --fishQualities[lowestFish];
                    ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                if (fishQualities.Sum() != pond.FishCount + 1)
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                _SetFishItem(info, new SObject(pond.fishType.Value, 1) {Quality = lowestFish == 3 ? 4 : lowestFish});
                _SetFishQuality(info, lowestFish == 3 ? 4 : lowestFish);
                --fishQualities[lowestFish];
                ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex) when (pond is not null)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.WriteData(pond, "FishQualities", $"{pond.FishCount},0,0,0");
            ModDataIO.WriteData(pond, "FamilyQualities", null);
            ModDataIO.WriteData(pond, "FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}