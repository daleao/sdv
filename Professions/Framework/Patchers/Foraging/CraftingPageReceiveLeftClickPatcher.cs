namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingPageReceiveLeftClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingPageReceiveLeftClickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CraftingPageReceiveLeftClickPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CraftingPage>(nameof(CraftingPage.receiveLeftClick));
    }

    #region harmony patches

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? CraftingPageReceiveLeftClickTranspiler(
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
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Keys.LeftShift),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);

            doNotSwap = generator.DefineLabel();
            resumeExecution = generator.DefineLabel();
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
        }
        catch (Exception ex)
        {
            Log.E($"Failed swapping crafting menu modifiers.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
