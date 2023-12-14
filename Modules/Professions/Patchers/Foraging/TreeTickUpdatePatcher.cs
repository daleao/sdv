namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var isPrestiged = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .MatchProfessionCheck(VanillaProfession.Lumberjack.Value)
                            .Move()
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, VanillaProfession.Lumberjack.Value + 100),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod(
                                            nameof(NetList<int, NetInt>.Contains))),
                                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged),
                                })
                            .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 1.25) })
                            .Move()
                            .AddLabels(resumeExecution)
                            .Insert(new[] { new CodeInstruction(OpCodes.Br_S, resumeExecution) })
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Pop),
                                    new CodeInstruction(OpCodes.Ldc_R8, 1.4),
                                },
                                new[] { isPrestiged });
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        // find the Arborist profession check
        CodeInstruction[] checkForArboristInstructions;
        try
        {
            helper
                .MatchProfessionCheck(VanillaProfession.Arborist.Value)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .CountUntil(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    },
                    out var steps)
                .Copy(
                    out checkForArboristInstructions,
                    steps,
                    true);
        }
        catch (Exception ex)
        {
            Log.E($"Failed getting instructions for Arborist check.\nHelper returned {ex}");
            return null;
        }

        // replace Arborist check for prestiged Arborist check
        var checkForPrestigedArboristInstructions = checkForArboristInstructions;
        checkForPrestigedArboristInstructions[5] =
            new CodeInstruction(OpCodes.Ldc_I4_S, VanillaProfession.Arborist.Value + 100);

        // From: numHardwood++;
        // To: numHardwood += Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 2 : 1;
        // -- and also
        // From: numHardwood += (int) (numHardwood * 0.25f + 0.9f);
        // To: numHardwood += (int) (numHardwood * (Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 0.5f : 0.25f) + 0.9f);
        helper.GoTo(0);
        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var notPrestigedArborist1 = generator.DefineLabel();
                        var notPrestigedArborist2 = generator.DefineLabel();
                        var resumeExecution1 = generator.DefineLabel();
                        var resumeExecution2 = generator.DefineLabel();
                        helper
                            .MatchProfessionCheck(VanillaProfession.Arborist.Value)
                            .Match(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldc_I4_1),
                                    new CodeInstruction(OpCodes.Add),
                                },
                                ILHelper.SearchOption.Previous)
                            .AddLabels(notPrestigedArborist1)
                            .Insert(checkForPrestigedArboristInstructions)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist1),
                                    new CodeInstruction(OpCodes.Ldc_I4_2),
                                    new CodeInstruction(OpCodes.Br_S, resumeExecution1),
                                })
                            .Move()
                            .AddLabels(resumeExecution1)
                            .MatchProfessionCheck(VanillaProfession.Arborist.Value)
                            .Match(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                                    new CodeInstruction(OpCodes.Mul),
                                })
                            .AddLabels(notPrestigedArborist2)
                            .Insert(checkForPrestigedArboristInstructions)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist2),
                                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                                    new CodeInstruction(OpCodes.Br_S, resumeExecution2),
                                })
                            .Move()
                            .AddLabels(resumeExecution2);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Arborist bonus hardwood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
