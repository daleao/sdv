namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class BobberBarUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BobberBarUpdatePatcher"/> class.</summary>
    internal BobberBarUpdatePatcher()
    {
        this.Target = this.RequireMethod<BobberBar>(nameof(BobberBar.update));
    }

    #region harmony patches

    /// <summary>Patch to slow-down catching bar decrease for Aquarist.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BobberBarUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        ///// From: distanceFromCatching += 0.002f;
        ///// To: distanceFromCatching +=  Game1.player.professions.Contains(100 + <fisher_id>)) ? GetFisherBonusCatchingBarSpeed() : 0.002f;

        //var isNotPrestigedFisher = generator.DefineLabel();
        //var resumeExecution = generator.DefineLabel();
        //try
        //{
        //    helper
        //        .FindFirst(
        //            new CodeInstruction(OpCodes.Ldc_R4, 0.002f)
        //        )
        //        .AddLabels(isNotPrestigedFisher)
        //        .InsertProfessionCheckForLocalPlayer(Profession.Fisher.Value + 100, isNotPrestigedFisher)
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
        //    Log.E($"Failed patching Fisher catching bar gain.\nHelper returned {ex}");
        //    return null;
        //}

        // Injected: if (Game1.player.professions.Contains(<aquarist_id>)) distanceFromCatching += Game1.player.GetAquaristCatchingBarCompensation();
        // After: distanceFromCatching -= ((whichBobber == 694 || beginnersRod) ? 0.002f : 0.003f);
        try
        {
            var isNotAquarist = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 694) })
                .Match(new[] { new CodeInstruction(OpCodes.Stfld) })
                .Move()
                .AddLabels(isNotAquarist)
                .InsertProfessionCheck(Profession.Aquarist.Value)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotAquarist), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions)
                                .RequireMethod(nameof(FarmerExtensions.GetAquaristCatchingBarCompensation))),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Aquarist catching bar loss.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
