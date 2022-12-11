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
internal sealed class TreePerformTreeFallPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreePerformTreeFallPatcher"/> class.</summary>
    internal TreePerformTreeFallPatcher()
    {
        this.Target = this.RequireMethod<Tree>("performTreeFall");
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreePerformTreeFallTranspiler(
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
                            .FindProfessionCheck(Profession.Lumberjack.Value)
                            .Move()
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Lumberjack.Value + 100),
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
                                new[] { new CodeInstruction(OpCodes.Pop), new CodeInstruction(OpCodes.Ldc_R8, 1.4), },
                                new[] { isPrestiged });
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
