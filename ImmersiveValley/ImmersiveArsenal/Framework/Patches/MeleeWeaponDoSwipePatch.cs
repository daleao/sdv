namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoSwipePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoSwipePatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.doSwipe));
    }

    #region harmony patches

    /// <summary>Inject stabby sword swipe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoSwipeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case 3:
        /// To: case 3 or 0:

        var isSword = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Bne_Un)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bne_Un)
                )
                .GetOperand(out var caseClub)
                .ReplaceInstructionWith(
                    new CodeInstruction(OpCodes.Beq_S, isSword)
                )
                .Advance()
                .AddLabels(isSword)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bne_Un, caseClub)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting stabby sword swipe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}