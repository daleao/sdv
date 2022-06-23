#nullable enable
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Common;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishProducePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondGetFishProducePatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.GetFishProduce));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool FishPondGetFishProducePrefix(FishPond __instance, ref SObject? __result, Random? random)
    {
        random ??= new(Guid.NewGuid().GetHashCode());

        try
        {
            var produce = new List<(int, int)>();
            SObject? output;
            if (__instance.IsAlgaePond())
            {
                var seaweedProduce = 0;
                for (var i = 0; i < __instance.ReadDataAs<int>("SeaweedLivingHere"); ++i)
                {
                    if (random.NextDouble() < StardewValley.Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                        ++seaweedProduce;
                }

                var greenAlgaeProduced = 0;
                for (var i = 0; i < __instance.ReadDataAs<int>("GreenAlgaeLivingHere"); ++i)
                {
                    if (random.NextDouble() < StardewValley.Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                        ++greenAlgaeProduced;
                }

                var whiteAlgaeProduced = 0;
                for (var i = 0; i < __instance.ReadDataAs<int>("WhiteAlgaeLivingHere"); ++i)
                {
                    if (random.NextDouble() < StardewValley.Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                        ++whiteAlgaeProduced;
                }

                if (seaweedProduce + greenAlgaeProduced + whiteAlgaeProduced == 0)
                    return false; // don't run original logic

                if (seaweedProduce > 0) produce.Add((Constants.SEAWEED_INDEX_I, seaweedProduce));
                if (greenAlgaeProduced > 0) produce.Add((Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced));
                if (whiteAlgaeProduced > 0) produce.Add((Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced));

                switch (__instance.fishType.Value)
                {
                    case Constants.SEAWEED_INDEX_I when seaweedProduce > 0:
                        output = new(Constants.SEAWEED_INDEX_I, seaweedProduce);
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I when greenAlgaeProduced > 0:
                        output = new(Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced);
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I when whiteAlgaeProduced > 0:
                        output = new(Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced);
                        break;
                    default:
                        if (seaweedProduce > 0 && seaweedProduce > greenAlgaeProduced &&
                            seaweedProduce > whiteAlgaeProduced)
                            output = new(Constants.SEAWEED_INDEX_I, seaweedProduce);
                        else if (greenAlgaeProduced > 0 && greenAlgaeProduced > seaweedProduce &&
                                 greenAlgaeProduced > whiteAlgaeProduced)
                            output = new(Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced);
                        else if (whiteAlgaeProduced > 0 && whiteAlgaeProduced > seaweedProduce &&
                                 whiteAlgaeProduced > greenAlgaeProduced)
                            output = new(Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced);
                        else output = null;
                        break;
                }

                if (output is not null) produce.Remove((output.ParentSheetIndex, output.Stack));

                if (produce.Any())
                {
                    var data = produce.Select(p => $"{p.Item1},{p.Item2},0");
                    __instance.WriteData("ItemsHeld", string.Join(';', data));
                }
            }
            else
            {
                var fishPondData = __instance.GetFishPondData();
                if (fishPondData is null)
                {
                    __result = null;
                    return false; // don't run original logic
                }

                foreach (var item in fishPondData.ProducedItems.Where(item =>
                             item.ItemID is not Constants.ROE_INDEX_I or Constants.SQUID_INK_INDEX_I &&
                             __instance.currentOccupants.Value >= item.RequiredPopulation &&
                             random.NextDouble() <
                             StardewValley.Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f) &&
                             random.NextDouble() < item.Chance))
                {
                    var stack = random.Next(item.MinQuantity, item.MaxQuantity + 1);
                    var existing = produce.FindIndex(p => p.Item1 == item.ItemID);
                    if (existing >= 0) produce[existing] = (item.ItemID, stack + produce[existing].Item2);
                    else produce.Add((item.ItemID, stack));
                }

                if (!produce.Any())
                {
                    __result = null;
                    return false; // don't run original logic
                }

                output = produce
                    .Select(p => new SObject(p.Item1, p.Item2))
                    .OrderByDescending(o => o.Price)
                    .First();
                produce.Remove((output.ParentSheetIndex, output.Stack));
                if (produce.Any())
                {
                    var data = produce.Select(p => $"{p.Item1},{p.Item2},0");
                    __instance.WriteData("ItemsHeld", string.Join(';', data));
                }
            }

            __result = output;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}