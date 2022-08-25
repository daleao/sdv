namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.BetterJunimos;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("hawkfalcon.BetterJunimos")]
internal sealed class PlantCropsAbilityApplySpeedIncreasesPatch : DaLion.Common.Harmony.HarmonyPatch
{
    internal PlantCropsAbilityApplySpeedIncreasesPatch()
    {
        Target = "BetterJunimos.Abilities.PlantCropsAbility".ToType().RequireMethod("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to apply prestiged Agriculturist crop growth bonus to Better Junimos.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PlantCropsAbilityApplySpeedIncreasesTranspiler(
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
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .InsertProfessionCheck(Profession.Agriculturist.Value + 100, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching prestiged Agriculturist crop growth bonus to Better Junimos." +
                  "\n—-- Do NOT report this to Better Junimos' author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}