namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDoFunctionPatcher"/> class.</summary>
    internal HoeDoFunctionPatcher()
    {
        this.Target = this.RequireMethod<Hoe>(nameof(Hoe.DoFunction));
    }

    #region harmony patches

    /// <summary>Apply base stamina multiplier + stamina cost cap.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? HoeDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: who.Stamina -= (float)(2 * power) - (float)who.<SkillLevel> * 0.1f;
        // To: who.Stamina -= Math.Max(((float)(2 * power) - (float)who.<SkillLevel> * 0.1f) * HoeConfig.BaseStaminaCostMultiplier, 0.1f);
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
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.Hoe))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(HoeConfig).RequirePropertyGetter(nameof(HoeConfig.BaseStaminaCostMultiplier))),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.1f),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding stamina cost multiplier and lower bound for the Hoe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
