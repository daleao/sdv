namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtApplySpeedIncreases : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal HoeDirtApplySpeedIncreases()
    {
        Target = RequireMethod<HoeDirt>("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Agriculturist crop growth speed.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? HoeDirtApplySpeedIncreasesTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += 0.1f;
        /// To: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += who.professions.Contains(100 + <agriculturist_id>)) ? 0.2f : 0.1f;

        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Profession.Agriculturist.Value)
                .Advance()
                .FindProfessionCheck(Profession.Agriculturist.Value, true)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f)
                )
                .AddLabels(isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1)
                )
                .InsertProfessionCheck(Profession.Agriculturist.Value + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Agriculturist bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}