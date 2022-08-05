namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerUpdateMovementAnimationPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerUpdateMovementAnimationPatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.updateMovementAnimation));
    }

    #region harmony patches

    /// <summary>Patch to allow Desperado movement during slingshot use.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerUpdateMovementAnimationTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (FarmerSprite.PauseForSingleAnimation || UsingTool)
        /// To: if (FarmerSprite.PauseForSingleAnimation || UsingTool && !this.IsDesperadoCharging)

        var i = 0;
    repeat:
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Call, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.UsingTool)))
                )
                .Advance()
                .GetOperand(out var dontRet)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsDesperadoCharging))),
                    new CodeInstruction(OpCodes.Brtrue_S, dontRet)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding movement exception for Desperado charging.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}