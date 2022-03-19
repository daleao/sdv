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

using Stardew.Common.Extensions;
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
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        ///// From: distanceFromCatching += 0.002f;
        ///// To: distanceFromCatching +=  Game1.player.professions.Contains(100 + <fished_id>)) ? GetFisherBonusCatchingBarSpeed() : 0.002f;

        //var isNotPrestigedFisher = generator.DefineLabel();
        //var resumeExecution = generator.DefineLabel();
        //try
        //{
        //    helper
        //        .FindFirst(
        //            new CodeInstruction(OpCodes.Ldc_R4, 0.002f)
        //        )
        //        .AddLabels(isNotPrestigedFisher)
        //        .InsertProfessionCheckForLocalPlayer((int) Profession.Fisher + 100, isNotPrestigedFisher)
        //        .InsertWithLabels(
        //            new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
        //            new CodeInstruction(OpCodes.Ldarg_0),
        //            new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).Field("whichFish")),
        //            new CodeInstruction(OpCodes.Call,
        //                typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions
        //                    .GetFisherBonusCatchingBarSpeed))),
        //            new CodeInstruction(OpCodes.Br_S, resumeExecution)
        //        )
        //        .Advance()
        //        .AddLabels(resumeExecution);
        //}
        //catch (Exception ex)
        //{
        //    Log.E($"Failed while patching Fisher catching bar gain. Helper returned {ex}");
        //    return null;
        //}

        /// Injected: if (Game1.player.professions.Contains(<aquarist_id>)) distanceFromCatching += Game1.player.GetAquaristCatchingBarCompensation();
        /// After: distanceFromCatching -= ((whichBobber == 694 || beginnersRod) ? 0.002f : 0.003f);

        var isNotAquarist = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, 694)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stfld)
                )
                .Advance()
                .AddLabels(isNotAquarist)
                .InsertProfessionCheck((int) Profession.Aquarist)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotAquarist),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).Field("distanceFromCatching")),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions
                            .GetAquaristCatchingBarCompensation))),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).Field("distanceFromCatching"))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Aquarist catching bar loss. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}