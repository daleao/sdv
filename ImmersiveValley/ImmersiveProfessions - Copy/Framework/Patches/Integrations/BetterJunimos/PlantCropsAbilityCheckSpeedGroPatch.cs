namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.BetterJunimos;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("hawkfalcon.BetterJunimos")]
internal sealed class PlantCropsAbilityCheckSpeedGroPatch : DaLion.Common.Harmony.HarmonyPatch
{
    internal PlantCropsAbilityCheckSpeedGroPatch()
    {
        Target = "BetterJunimos.Abilities.PlantCropsAbility".ToType().RequireMethod("CheckSpeedGro");
    }

    #region harmony patches

    /// <summary>Patch to apply prestiged Agriculturist crop growth bonus to Better Junimos.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PlantCropsAbilityCheckSpeedGroTranspiler(
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
                    new CodeInstruction(OpCodes.Ldloc_1)
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
            Log.E($"Failed while patching prestiged Agriculturist crop growth bonus to Better Junimos.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}