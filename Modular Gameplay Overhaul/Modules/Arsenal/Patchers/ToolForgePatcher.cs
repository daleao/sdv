namespace DaLion.Overhaul.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches

    /// <summary>Invalidate stats on forge.</summary>
    [HarmonyPostfix]
    private static void ToolForgePostfix(Tool __instance, bool __result)
    {
        if (__result)
        {
            __instance.Invalidate();
        }
    }

    #endregion harmony patches
}
