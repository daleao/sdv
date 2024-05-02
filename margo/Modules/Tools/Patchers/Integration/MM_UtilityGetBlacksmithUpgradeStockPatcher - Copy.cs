namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.MoonMisadventures", "Moon Misadventure")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class ModOnMenuChangedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ModOnMenuChangedPatcher"/> class.</summary>
    internal ModOnMenuChangedPatcher()
    {
        this.Target = "MoonMisadventures.Mod".ToType().RequireMethod("OnMenuChanged");
    }

    #region harmony patches

    /// <summary>Prevents Radioactive upgrades at Clint's.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ModOnMenuChangedTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) })
                .GetOperand(out var skipShopMenu)
                .GoTo(0)
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
                            typeof(ToolConfig).RequirePropertyGetter(nameof(ToolConfig.EnableForgeUpgrading))),
                        new CodeInstruction(OpCodes.Brtrue, skipShopMenu),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed to remove Moon mod tool upgrades from Clint.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
