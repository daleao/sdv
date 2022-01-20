namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class BobberBarUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BobberBarUpdatePatch()
    {
        Original = RequireMethod<BobberBar>(nameof(BobberBar.update));
    }

    #region harmony patches

    /// <summary>Patch to slow-down catching bar decrease for Aquarist.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BobberBarUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<aquarist_id>) distanceFromCatching -= GetAquaristBonusCatchingBarSpeed();
        /// After: distanceFromCatching -= ((whichBobber == 694 || beginnersRod) ? 0.002f : 0.003f);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, 694)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var resumeExecution)
                .Return()
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stfld)
                )
                .Advance()
                .InsertProfessionCheckForLocalPlayer((int) Profession.Aquarist, (Label) resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).Field("distanceFromCatching")),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions
                            .GetAquaristBonusCatchingBarSpeed))),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).Field("distanceFromCatching"))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Aquarist catching bar loss. Helper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}