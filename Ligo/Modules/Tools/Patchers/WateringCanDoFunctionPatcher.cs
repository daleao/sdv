namespace DaLion.Ligo.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Tools.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class WateringCanDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="WateringCanDoFunctionPatcher"/> class.</summary>
    internal WateringCanDoFunctionPatcher()
    {
        this.Target = this.RequireMethod<WateringCan>(nameof(WateringCan.DoFunction));
    }

    #region harmony patches

    /// <summary>Apply base stamina multiplier.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? WateringCanDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: who.Stamina -= (float)(2 * power) - (float)who.<SkillLevel> * 0.1f;
        // To: who.Stamina -= Math.Max(((float)(2 * power) - (float)who.<SkillLevel> * 0.1f) * WateringCanConfig.BaseStaminaMultiplier, 0.1f);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ToolsConfig).RequirePropertyGetter(nameof(ToolsConfig.Can))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(WateringCanConfig).RequirePropertyGetter(
                                nameof(WateringCanConfig.BaseStaminaMultiplier))),
                        new CodeInstruction(OpCodes.Mul), new CodeInstruction(OpCodes.Ldc_R4, 1f), new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding stamina cost multiplier and lower bound for the Watering Can.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
