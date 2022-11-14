namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches

    /// <summary>Require hero soul to transform galaxy into infinity.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolForgeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Isinst, typeof(GalaxySoulEnchantment)))
                .SetOperand(typeof(InfinityEnchantment))
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.GetEnchantmentOfType))
                            .MakeGenericMethod(typeof(GalaxySoulEnchantment))))
                .StripLabels(out var labels)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var toRemove)
                .Return()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.RemoveEnchantment))))
                .RemoveLabels((Label)toRemove)
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting hero soul condition for Infinity Blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
