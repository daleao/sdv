using System;
using StardewValley;
using StardewValley.GameData.FishPond;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDayUpdatePatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to boost roe production for everybody.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance, ref FishPondData ____fishPondData)
    {
        if (!ModEntry.Config.RebalanceFishPonds) return;

        if (__instance.currentOccupants.Value == 0)
        {
            __instance.IncrementData<int>("DaysEmpty");
            if (__instance.ReadDataAs<int>("DaysEmpty") < 3) return;

            var r = new Random(Guid.NewGuid().GetHashCode());
            __instance.fishType.Value = r.NextDouble() < 0.25 ? 157 : r.Next(152, 154);
            ____fishPondData = null;
            __instance.UpdateMaximumOccupancy();
            ++__instance.currentOccupants.Value;
            __instance.WriteData("DaysEmpty", 0.ToString());
        }
        else
        {
            __instance.AddBonusProduceAmountAndQuality();
        }
    }

    #endregion harmony patches
}