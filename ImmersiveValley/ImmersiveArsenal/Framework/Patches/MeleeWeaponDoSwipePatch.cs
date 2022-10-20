namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoSwipePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoSwipePatch"/> class.</summary>
    internal MeleeWeaponDoSwipePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.doSwipe));
    }

    #region harmony patches

    /// <summary>Allow swiping stabby sword.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoSwipeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: case 3:
        // To: case 3 or 0:
        try
        {
            var isSword = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Bne_Un))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bne_Un))
                .GetOperand(out var caseClub)
                .ReplaceInstructionWith(
                    new CodeInstruction(OpCodes.Beq_S, isSword))
                .Advance()
                .AddLabels(isSword)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bne_Un, caseClub));
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
