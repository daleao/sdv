namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
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
    internal FishPondDayUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Reset held items each morning.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static void FishPondDayUpdatePrefix(FishPond __instance)
    {
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
    private static void FishPondDayUpdatePostfix(FishPond __instance, ref FishPondData? ____fishPondData)
    {
        if (!string.IsNullOrEmpty(__instance.fishType.Value) || __instance.currentOccupants.Value != 0)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());

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

    /// <summary>
    ///     Removes population-based roll from <see cref="FishPond.dayUpdate"/> (moved to
    ///     <see cref="FishPond.GetFishProduce"/>).
    /// </summary>
    private static IEnumerable<CodeInstruction>? FishPondDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Random).RequireMethod(nameof(Random.NextDouble))),
                ])
                .RemoveUntil([
                    new CodeInstruction(OpCodes.Blt_S),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(FishPond).RequireField(nameof(FishPond.goldenAnimalCracker))),
                ])
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing day update production roll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
