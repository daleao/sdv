namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class HoeDirtApplySpeedIncreases : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal HoeDirtApplySpeedIncreases()
    {
        Original = RequireMethod<HoeDirt>("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Agriculturist crop growth speed.</summary>
    [HarmonyTranspiler]
    protected static IEnumerable<CodeInstruction> HoeDirtApplySpeedIncreasesTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(100 + <agriculturist_id>)) speedIncrease += 0.1f;

        var notPrestigedAgriculturist = iLGenerator.DefineLabel();
        var resumeExecution = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck("Agriculturist".ToProfessionIndex())
                .Advance()
                .FindProfessionCheck("Agriculturist".ToProfessionIndex())
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f)
                )
                .AddLabels(notPrestigedAgriculturist)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1)
                )
                .InsertProfessionCheckForPlayerOnStack("Agriculturist".ToProfessionIndex() + 100,
                    notPrestigedAgriculturist)
                .Insert(
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