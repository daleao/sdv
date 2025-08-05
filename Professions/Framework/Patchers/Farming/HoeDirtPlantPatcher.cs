namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtPlantPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtPlantPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal HoeDirtPlantPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.plant));
    }

    #region harmony patches

    /// <summary>Patch to record crops planted by Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void HoeDirtPlantPostfix(HoeDirt __instance, ref bool __result, string itemId, Farmer who, bool isFertilizer)
    {
        if (!__result || !who.HasProfessionOrLax(Profession.Agriculturist, true) || __instance.crop is not { } crop)
        {
            return;
        }

        var retention = __instance.GetFertilizerWaterRetentionChance();
        var daysLeftOutOfSeason = retention switch
        {
            < 0.01f => 0,
            < 0.34f => 3,
            < 0.67f => 7,
            _ => 13,
        };

        if (daysLeftOutOfSeason > 0)
        {
            Data.Write(crop, DataKeys.DaysLeftOutOfSeason, daysLeftOutOfSeason.ToString());
            __instance.fertilizer.Value = null;
        }

        __result = true;
    }

    /// <summary>Patch for Prestiged Agriculturist crop endurance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? HoeDirtPlantTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var testRetentionSoil = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(GameLocation).RequireMethod(nameof(GameLocation.SeedsIgnoreSeasonsHere))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse)])
                .GetOperand(out var falseBranch)
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)])
                .GetLabels(out var trueLabels)
                .Return()
                .SetOpCode(OpCodes.Brfalse_S)
                .SetOperand(testRetentionSoil)
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse)])
                .SetOpCode(OpCodes.Brtrue_S)
                .SetOperand(trueLabels[0])
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(HoeDirtPlantPatcher).RequireMethod(
                                nameof(CanGrowWithRetainingSoil))),
                        new CodeInstruction(OpCodes.Brfalse_S, falseBranch)
                    ],
                    [testRetentionSoil]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting crop retention.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool CanGrowWithRetainingSoil(HoeDirt dirt, Farmer who)
    {
        return who.HasProfession(Profession.Agriculturist, true) &&
               dirt.GetFertilizerWaterRetentionChance() > 0f && dirt.Location.GetSeason() != Season.Winter;
    }

    #endregion injected
}
