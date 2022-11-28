namespace DaLion.Ligo.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeTickUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeTickUpdatePatcher"/> class.</summary>
    internal TreeTickUpdatePatcher()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.tickUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreeTickUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        // To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0
        var i = 0;
        repeat1:
        try
        {
            var isPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindProfessionCheck(Profession.Lumberjack.Value, true)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Lumberjack.Value + 100),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_R8, 1.25))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertInstructions(new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { isPrestiged },
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Ldc_R8, 1.4));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2)
        {
            goto repeat1;
        }

        // find the Arborist profession check
        CodeInstruction[] checkForArboristInstructions;
        try
        {
            helper
                .FindProfessionCheck(Profession.Arborist.Value, true)
                .RetreatUntil(new CodeInstruction(OpCodes.Ldarg_0))
                .GetInstructionsUntil(
                    out checkForArboristInstructions,
                    true,
                    false,
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while getting instructions for Arborist check.\nHelper returned {ex}");
            return null;
        }

        // replace Arborist check for prestiged Arborist check
        var checkForPrestigedArboristInstructions = checkForArboristInstructions;
        checkForPrestigedArboristInstructions[5] =
            new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Arborist.Value + 100);

        // From: numHardwood++;
        // To: numHardwood += Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 2 : 1;
        // -- and also
        // From: numHardwood += (int) (numHardwood * 0.25f + 0.9f);
        // To: numHardwood += (int) (numHardwood * (Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 0.5f : 0.25f) + 0.9f);
        helper.GoTo(0);
        i = 0;
        repeat2:
        try
        {
            var notPrestigedArborist1 = generator.DefineLabel();
            var notPrestigedArborist2 = generator.DefineLabel();
            var resumeExecution1 = generator.DefineLabel();
            var resumeExecution2 = generator.DefineLabel();
            helper
                .FindProfessionCheck(Profession.Arborist.Value, true)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add))
                .AddLabels(notPrestigedArborist1)
                .InsertInstructions(checkForPrestigedArboristInstructions)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist1),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1))
                .Advance()
                .AddLabels(resumeExecution1)
                .FindProfessionCheck(Profession.Arborist.Value, true)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                    new CodeInstruction(OpCodes.Mul))
                .AddLabels(notPrestigedArborist2)
                .InsertInstructions(checkForPrestigedArboristInstructions)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist2),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution2))
                .Advance()
                .AddLabels(resumeExecution2);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Arborist bonus hardwood.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2)
        {
            goto repeat2;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
