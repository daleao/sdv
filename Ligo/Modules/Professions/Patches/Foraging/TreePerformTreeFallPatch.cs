namespace DaLion.Ligo.Modules.Professions.Patches.Foraging;

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
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TreePerformTreeFallPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TreePerformTreeFallPatch"/> class.</summary>
    internal TreePerformTreeFallPatch()
    {
        this.Target = this.RequireMethod<Tree>("performTreeFall");
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreePerformTreeFallTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        // To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0
        var i = 0;
        repeat:
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
            goto repeat;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
