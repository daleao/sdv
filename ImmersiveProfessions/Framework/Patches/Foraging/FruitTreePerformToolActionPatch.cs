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

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FruitTreePerformToolAction : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreePerformToolAction()
    {
        Original = RequireMethod<FruitTree>(nameof(FruitTree.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FruitTreePerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        /// To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0

        var i = 0;
        repeat:
        try
        {
            var isPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindProfessionCheck((int) Profession.Lumberjack, true)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) Profession.Lumberjack + 100),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged)
                )
                .AdvanceUntil(
                    i > 0
                        ? new(OpCodes.Ldc_R8, 1.25)
                        : new CodeInstruction(OpCodes.Ldc_I4_5)
                )
                .Advance()
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] {isPrestiged},
                    new CodeInstruction(OpCodes.Pop),
                    i > 0
                        ? new(OpCodes.Ldc_R8, 1.4)
                        : new CodeInstruction(OpCodes.Ldc_I4_6)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}