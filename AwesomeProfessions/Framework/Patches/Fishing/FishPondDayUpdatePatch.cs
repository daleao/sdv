using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using TheLion.Stardew.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class FishPondDayUpdatePatch : BasePatch
{
    private const int ROE_INDEX_I = 812;
    private const int SQUID_INK_INDEX_I = 814;
    private const int SEAWEED_INDEX_I = 152;
    private const int ALGAE_INDEX_I = 153;

    private static readonly Func<int, double> _productionChanceByValue = x => (double) 14765 / (x + 120) + 1.5;

    /// <summary>Construct an instance.</summary>
    internal FishPondDayUpdatePatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to boost roe production for everybody.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance)
    {
        if (!ModEntry.Config.EnableFishPondRebalance) return;

        var produce = __instance.output.Value as SObject;
        if (produce is not null && !produce.ParentSheetIndex.IsAnyOf(ROE_INDEX_I, SQUID_INK_INDEX_I)) return;

        var r = new Random(Guid.NewGuid().GetHashCode());
        var fish = __instance.GetFishObject();
        var bonusStack = 0;
        var productionChancePerFish = _productionChanceByValue(fish.Price) / 100;
        for (var i = 0; i < __instance.currentOccupants.Value; ++i)
            if (r.NextDouble() < productionChancePerFish)
                ++bonusStack;

        if (bonusStack <= 0) return;

        if (produce is null)
        {
            int produceId;
            if (fish.Name.Contains("Squid"))
                produceId = SQUID_INK_INDEX_I;
            else if (fish.Name != "Coral")
                produceId = ROE_INDEX_I;
            else
                produceId = r.Next(SEAWEED_INDEX_I, ALGAE_INDEX_I + 1);

            if (produceId != ROE_INDEX_I)
            {
                produce = new(produceId, bonusStack);
            }
            else
            {
                var split = Game1.objectInformation[__instance.fishType.Value].Split('/');
                var c = TailoringMenu.GetDyeColor(__instance.GetFishObject()) ??
                        (__instance.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
                produce = new ColoredObject(ROE_INDEX_I, 1, c);
                produce.name = split[0] + " Roe";
                produce.preserve.Value = SObject.PreserveType.Roe;
                produce.preservedParentSheetIndex.Value = __instance.fishType.Value;
                produce.Price += Convert.ToInt32(split[1]) / 2;
            }
        }
        else
        {
            produce.Stack += bonusStack;
        }

        __instance.output.Value = produce;
    }

    #endregion harmony patches
}