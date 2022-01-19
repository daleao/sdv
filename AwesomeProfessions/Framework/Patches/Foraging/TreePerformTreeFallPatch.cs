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
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class TreePerformTreeFallPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreePerformTreeFallPatch()
    {
        Original = RequireMethod<Tree>("performTreeFall");
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TreePerformTreeFallTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        /// To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0

        var i = 0;
        repeat:
        try
        {
            var isPrestiged = iLGenerator.DefineLabel();
            var resumeExecution = iLGenerator.DefineLabel();
            helper
                .FindProfessionCheck("Lumberjack".ToProfessionIndex(), true)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_S, "Lumberjack".ToProfessionIndex() + 100),
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

        // repeat injection
        if (++i < 2) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}