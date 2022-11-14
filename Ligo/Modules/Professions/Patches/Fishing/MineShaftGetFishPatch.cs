namespace DaLion.Ligo.Modules.Professions.Patches.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftGetFishPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftGetFishPatch"/> class.</summary>
    internal MineShaftGetFishPatch()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (Game1.player.professions.Contains(<fisher_id>)) <baseChance> *= 2
        // Before each of the three fish rolls
        var i = 0;
        repeat:
        try
        {
            var isNotFisher = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble))))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_R8, 0.02 - (i * 0.005)))
                .Advance()
                .AddLabels(isNotFisher)
                .InsertProfessionCheck(Profession.Fisher.Value)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                    new CodeInstruction(OpCodes.Mul));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        if (++i < 3)
        {
            goto repeat;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
