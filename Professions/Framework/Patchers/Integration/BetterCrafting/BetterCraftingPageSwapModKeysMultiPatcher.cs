namespace DaLion.Professions.Framework.Patchers.Integration.BetterCrafting;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion using directives

[UsedImplicitly]
[ModRequirement("leclair.bettercrafting", minimumVersion: "2.18.0")]
internal sealed class BetterCraftingPageSwapModKeysMultiPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingPageSwapModKeysMultiPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BetterCraftingPageSwapModKeysMultiPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
    }

    protected override bool ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            Log.D($"Patching {target.Name} method...");
            this.Target = target;
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return
        [
            "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
                .ToType()
                .RequireMethod("draw", [typeof(SpriteBatch)]),
            "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
                .ToType()
                .RequireMethod("GetRecipeTooltip"),
            "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
                .ToType()
                .RequireMethod("OpenBulkCraft"),
            "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
                .ToType()
                .RequireMethod("PerformAction"),
        ];
    }

    #region harmony patches

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? BetterCraftingPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var doNotSwap = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Keys.LeftShift),
                ])
                .AddLabels(doNotSwap)
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ProfessionsConfig).RequirePropertyGetter(
                            nameof(ProfessionsConfig.ModKey))),
                    new CodeInstruction(OpCodes.Ldc_I4, (int)SButton.LeftShift),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(KeybindListExtensions).RequireMethod(nameof(KeybindListExtensions.HasButton))),
                    new CodeInstruction(OpCodes.Brfalse_S, doNotSwap),
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Keys.LeftControl),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);

            doNotSwap = generator.DefineLabel();
            resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Keys.LeftControl),
                ])
                .AddLabels([doNotSwap])
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ProfessionsConfig).RequirePropertyGetter(
                            nameof(ProfessionsConfig.ModKey))),
                    new CodeInstruction(OpCodes.Ldc_I4, (int)SButton.LeftShift),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(KeybindListExtensions).RequireMethod(nameof(KeybindListExtensions.HasButton))),
                    new CodeInstruction(OpCodes.Brfalse_S, doNotSwap),
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Keys.LeftShift),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed swapping crafting menu modifiers for Better Crafting menu.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
