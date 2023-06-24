namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class BobberBarCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BobberBarCtorPatcher"/> class.</summary>
    internal BobberBarCtorPatcher()
    {
        this.Target = this.RequireConstructor<BobberBar>(typeof(int), typeof(float), typeof(bool), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch for Angler rod memory.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BobberBarCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);
        var readMethod = typeof(ItemExtensions).GetMethods()
            .FirstOrDefault(mi =>
                mi.Name.Contains(nameof(ItemExtensions.Read)) && mi.GetGenericArguments().Length > 0)
            ?.MakeGenericMethod(typeof(int)) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");

        try
        {
            var doesHaveQualityBobber = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 877) })
                .Move()
                .GetOperand(out var resumeExecution)
                .ReplaceWith(new CodeInstruction(OpCodes.Beq_S, doesHaveQualityBobber))
                .Move()
                .AddLabels(doesHaveQualityBobber)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 877),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Quality Bobber memory.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var doesHaveCorkBobber = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 695) })
                .Move()
                .GetOperand(out var resumeExecution)
                .ReplaceWith(new CodeInstruction(OpCodes.Beq_S, doesHaveCorkBobber))
                .Move()
                .AddLabels(doesHaveCorkBobber)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 695),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Cork Bobber memory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
