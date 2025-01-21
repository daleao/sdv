namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Reset held items each morning.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [UsedImplicitly]
    private static void FishPondDayUpdatePrefix(FishPond __instance, ref Item? __state)
    {
        __state = __instance.output.Value;
        if (!__instance.IsRadioactive())
        {
            return;
        }

        var heldMetals =
            Data.Read(__instance, DataKeys.MetalsHeld)
                .ParseList<string>(';')
                .Select(li => li?.ParseTuple<string, int>())
                .WhereNotNull()
                .ToList();
        for (var i = 0; i < heldMetals.Count; i++)
        {
            var (metal, daysLeft) = heldMetals[i];
            heldMetals[i] = (metal, --daysLeft);
        }

        Data.Write(
            __instance,
            DataKeys.MetalsHeld,
            string.Join(';', heldMetals.Select(m => string.Join(',', m.Item1, m.Item2))));
    }

    /// <summary>Spontaneously grow algae.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishPondDayUpdatePostfix(FishPond __instance, Item __state, ref FishPondData? ____fishPondData)
    {
        var r = Utility.CreateDaySaveRandom(__instance.tileX.Value * 1000, __instance.tileY.Value * 2000);
        // if pond is not empty, override output
        if (__instance.currentOccupants.Value > 0 && ____fishPondData is not null)
        {
            __instance.output.Value = __state;
            __instance.output.Value = __instance.GetProduce(r);
            return;
        }

        if (!string.IsNullOrEmpty(__instance.fishType.Value) || __instance.currentOccupants.Value != 0)
        {
            return;
        }

        // if pond is empty, spontaneously grow algae/seaweed
        Data.Increment(__instance, DataKeys.DaysEmpty);
        if (Data.ReadAs<uint>(__instance, DataKeys.DaysEmpty) < Config.DaysUntilAlgaeSpawn + 1)
        {
            return;
        }

        var spawned = new PondFish(r.NextAlgae(), SObject.lowQuality);
        __instance.fishType.Value = spawned.Id;
        ____fishPondData = null;
        __instance.UpdateMaximumOccupancy();
        __instance.currentOccupants.Value++;
        Data.Append(__instance, DataKeys.PondFish, spawned.ToString(), ';');
        Data.Write(__instance, DataKeys.DaysEmpty, null);
    }

    #endregion harmony patches
}
