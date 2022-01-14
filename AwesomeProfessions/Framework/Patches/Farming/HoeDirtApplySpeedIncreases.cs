namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

using Stardew.Common.Harmony;

using Professions = Utility.Professions;

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
                .FindProfessionCheck(Professions.IndexOf("Agriculturist"))
                .Advance()
                .FindProfessionCheck(Professions.IndexOf("Agriculturist"))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f)
                )
                .AddLabels(notPrestigedAgriculturist)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1)
                )
                .InsertProfessionCheckForPlayerOnStack(100 + Professions.IndexOf("Agriculturist"),
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
            ModEntry.Log($"Failed while patching prestiged Agriculturist bonus.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}