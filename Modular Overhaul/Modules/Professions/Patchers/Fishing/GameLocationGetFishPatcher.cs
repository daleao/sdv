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
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetFishPatcher"/> class.</summary>
    internal GameLocationGetFishPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.getFish));
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        var types = new[]
        {
            typeof(GameLocation), typeof(Beach), typeof(Mountain), typeof(Town), typeof(MineShaft), typeof(Sewer),
            typeof(Submarine),
        };

        foreach (var type in types)
        {
            this.Target = type.RequireMethod("getFish");
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        var types = new[]
        {
            typeof(GameLocation), typeof(Beach), typeof(Mountain), typeof(Town), typeof(MineShaft), typeof(Sewer),
            typeof(Submarine),
        };

        foreach (var type in types)
        {
            this.Target = type.RequireMethod("getFish");
            if (!base.UnapplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    #region harmony patches

    /// <summary>Patch for Angler rod memory.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var readMethod = typeof(ItemExtensions).GetMethods()
                .FirstOrDefault(mi =>
                    mi.Name.Contains(nameof(ItemExtensions.Read)) && mi.GetGenericArguments().Length > 0)
                ?.MakeGenericMethod(typeof(int)) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");

            var doesHaveCuriosityLure = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 856) })
                .Move()
                .GetOperand(out var resumeExecution)
                .ReplaceWith(new CodeInstruction(OpCodes.Beq_S, doesHaveCuriosityLure))
                .Move()
                .AddLabels(doesHaveCuriosityLure)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 856),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Curiosity Lure memory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
