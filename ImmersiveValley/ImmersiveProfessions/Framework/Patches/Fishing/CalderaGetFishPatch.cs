namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class CaderaGetFishPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CaderaGetFishPatch()
    {
        Target = RequireMethod<Caldera>(nameof(Caldera.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<fisher_id>)) <baseChance> *= 2 
        ///	After: if (Game1.random.NextDouble() < 0.1 ...

        try
        {
            var isNotFisher = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.1)
                )
                .Advance()
                .AddLabels(isNotFisher)
                .InsertProfessionCheck(Profession.Fisher.Value)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                    new CodeInstruction(OpCodes.Mul)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}