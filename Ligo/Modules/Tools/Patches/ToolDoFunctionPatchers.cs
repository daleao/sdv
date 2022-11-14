namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDoFunctionPatchers : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDoFunctionPatchers"/> class.</summary>
    internal ToolDoFunctionPatchers()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.DoFunction));
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.ApplyImpl(harmony);
        }
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(Axe).RequireMethod(nameof(Axe.DoFunction));
        yield return typeof(Hoe).RequireMethod(nameof(Hoe.DoFunction));
        yield return typeof(Pickaxe).RequireMethod(nameof(Pickaxe.DoFunction));
        yield return typeof(WateringCan).RequireMethod(nameof(WateringCan.DoFunction));
    }

    #region harmony patches

    /// <summary>Add hard lower-bound to stamina cost.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: who.Stamina -= (float)(2 * power) - (float)who.<SkillLevel> * 0.1f;
        // To: who.Stamina -= Math.Max((float)(2 * power) - (float)who.<SkillLevel> * 0.1f, 0.1f);
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina))))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldc_R4, 1f),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })));
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting stamina cost lower bound.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
