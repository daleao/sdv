namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
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

        //// Injected: if (Game1.player.professions.Contains(<aquarist_id>)) distanceFromCatching += Game1.player.GetAquaristCatchingBarCompensation();
        //// After: distanceFromCatching += 0.002f;
        //try
        //{
        //    var isNotAquarist = generator.DefineLabel();
        //    helper
        //        .Match(new[] { new CodeInstruction(OpCodes.Ldc_R4, 0.002f) })
        //        .Move()
        //        .AddLabels(isNotAquarist)
        //        .InsertProfessionCheck(Profession.Aquarist.Value)
        //        .Insert(
        //            new[]
        //            {
        //                new CodeInstruction(OpCodes.Brfalse_S, isNotAquarist),
        //                new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
        //                new CodeInstruction(
        //                    OpCodes.Call,
        //                    typeof(Extensions.FarmerExtensions)
        //                        .RequireMethod(nameof(Extensions.FarmerExtensions.GetAquaristCatchingHandicap))),
        //                new CodeInstruction(OpCodes.Add),
        //            });
        //}
        //catch (Exception ex)
        //{
        //    Log.E($"Failed buffing Aquarist catching bar gain.\nHelper returned {ex}");
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
                        new CodeInstruction(OpCodes.Brfalse_S, isNotAquarist),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Extensions.FarmerExtensions)
                                .RequireMethod(nameof(Extensions.FarmerExtensions.GetAquaristCatchingHandicap))),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Aquarist catching bar loss.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isNotFisher = generator.DefineLabel();
            var isNotPrestigedFisher = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 0.25) }, ILHelper.SearchOption.First)
                .Move()
                .AddLabels(isNotFisher, isNotPrestigedFisher)
                .InsertProfessionCheck(Farmer.fisher)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.25),
                        new CodeInstruction(OpCodes.Add),
                    })
                .InsertProfessionCheck(Farmer.fisher + 100)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestigedFisher),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.25),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling Wild Bait effect.\nHelper returned {ex}");
            return null;
        }

        helper.GoTo(0);
        var readMethod = typeof(ItemExtensions).GetMethods()
            .FirstOrDefault(mi =>
                mi.Name.Contains(nameof(ItemExtensions.Read)) && mi.GetGenericArguments().Length > 0)
            ?.MakeGenericMethod(typeof(int)) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 691) })
                .Move()
                .GetOperand(out var doesHaveBarbedHook)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 691),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveBarbedHook),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Barbed Hook memory (1).\nHelper returned {ex}");
            return null;
        }

        try
        {
            var doesHaveBarbedHook = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 691) })
                .Move()
                .GetOperand(out var resumeExecution)
                .ReplaceWith(new CodeInstruction(OpCodes.Beq_S, doesHaveBarbedHook))
                .Move()
                .AddLabels(doesHaveBarbedHook)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 691),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Barbed Hook memory (2).\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 692) })
                .Move()
                .GetOperand(out var doesHaveLeadBobber)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 692),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveLeadBobber),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Lead Bobber memory.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 693) })
                .Move()
                .GetOperand(out var doesHaveTreasureHunter)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 693),
                        new CodeInstruction(OpCodes.Beq, doesHaveTreasureHunter),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Treasure Hunter memory.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 694) })
                .Move()
                .GetOperand(out var doesHaveTrapBobber)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 694),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveTrapBobber),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Trap Bobber memory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
