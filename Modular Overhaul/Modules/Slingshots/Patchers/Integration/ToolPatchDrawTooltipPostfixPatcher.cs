namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ToolPatchDrawTooltipPostfixPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolPatchDrawTooltipPostfixPatcher"/> class.</summary>
    internal ToolPatchDrawTooltipPostfixPatcher()
    {
        this.Target = "Archery.Framework.Patches.Objects.ToolPatch"
            .ToType()
            .RequireMethod("DrawTooltipPostfix");
    }

    #region harmony patches

    /// <summary>Override tooltip damage.</summary>
    [HarmonyPrefix]
    private static bool ToolPatchDrawTooltipPostfixPrefix()
    {
        return false; // don't run original logic
    }

    #endregion harmony patches
}
