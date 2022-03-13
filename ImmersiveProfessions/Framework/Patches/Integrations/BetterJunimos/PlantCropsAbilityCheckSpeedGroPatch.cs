namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.BetterJunimos;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class PlantCropsAbilityCheckSpeedGroPatch : BasePatch
{
    internal PlantCropsAbilityCheckSpeedGroPatch()
    {
        try
        {
            Original = "BetterJunimos.Abilities.PlantCropsAbility".ToType().MethodNamed("CheckSpeedGro");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply prestiged Agriculturist crop growth bonus to Better Junimos.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PlantCropsAbilityCheckSpeedGroTranspiler(
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
                .FindProfessionCheck((int) Profession.Agriculturist)
                .Advance()
                .FindProfessionCheck((int) Profession.Agriculturist, true)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f)
                )
                .AddLabels(isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_1)
                )
                .InsertProfessionCheck((int) Profession.Agriculturist + 100, forLocalPlayer: false)
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
            Log.E($"Failed while patching prestiged Agriculturist crop growth bonus to Better Junimos.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}