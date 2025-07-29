namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtApplySpeedIncreasesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtApplySpeedIncreasesPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal HoeDirtApplySpeedIncreasesPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<HoeDirt>("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Agriculturist crop growth speed.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? HoeDirtApplySpeedIncreasesTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += 0.1f;
        // To: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += GetWhispererMultiplier(dirt);
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Profession.Agriculturist.Value)
                .Move()
                .MatchProfessionCheck(Profession.Agriculturist.Value)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R4, 0.1f)])
                .Remove()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, typeof(HoeDirtApplySpeedIncreasesPatcher).RequireMethod(nameof(GetAgriculturistMultiplier))),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching prestiged Agriculturist bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static float GetAgriculturistMultiplier(HoeDirt dirt, Farmer who)
    {
        if (!who.HasProfessionOrLax(Profession.Agriculturist))
        {
            return 0f;
        }

        var cropMemory = Data.Read(dirt.crop, DataKeys.SoilMemory);
        var stacks = 0;
        if (!string.IsNullOrEmpty(cropMemory))
        {
            stacks = cropMemory.Count(c => c == ',') + 1;
        }

        var multiplier = stacks * 0.05f;
        Log.D($"Applied a {multiplier}x speed multiplier.");
        return multiplier;
    }

    #endregion injected
}
