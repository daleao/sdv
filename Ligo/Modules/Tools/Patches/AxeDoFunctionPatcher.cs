namespace DaLion.Ligo.Modules.Tools.Patches;

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
internal sealed class AxeDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AxeDoFunctionPatcher"/> class.</summary>
    internal AxeDoFunctionPatcher()
    {
        this.Target = this.RequireMethod<Axe>(nameof(Axe.DoFunction));
    }

    #region harmony patches

    /// <summary>Apply base stamina multiplier.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? AxeDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: who.Stamina -= (float)(2 * power) - (float)who.<SkillLevel> * 0.1f;
        // To: who.Stamina -= Math.Max(((float)(2 * power) - (float)who.<SkillLevel> * 0.1f) * AxeConfig.BaseStaminaMultiplier, 0.1f);
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina))))
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.Axe))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(AxeConfig).RequirePropertyGetter(nameof(AxeConfig.BaseStaminaMultiplier))),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Ldc_R4, 1f),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding stamina cost multiplier and lower bound for the Axe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void AxeDoFunctionPostfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max((2 * (power + 1)) - (who.ForagingLevel * 0.1f), 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(ModEntry.Config.Tools.StaminaCostMultiplier, power);
    }

    #endregion harmony patches
}
