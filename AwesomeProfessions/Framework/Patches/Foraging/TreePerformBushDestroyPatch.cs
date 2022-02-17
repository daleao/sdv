namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley.TerrainFeatures;

using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class TreePerformBushDestroy : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreePerformBushDestroy()
    {
        Original = RequireMethod<Tree>("performBushDestroy");
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TreePerformBushDestroyTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        /// To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0

        var isPrestiged = iLGenerator.DefineLabel();
        var resumeExecution = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Lumberjack, true)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) Profession.Lumberjack + 100),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains))),
                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R8, 1.25)
                )
                .Advance()
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Insert(
                    new[] {isPrestiged},
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Ldc_R8, 1.4)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}