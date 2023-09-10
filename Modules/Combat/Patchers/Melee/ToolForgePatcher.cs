namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

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
        if (__instance is MeleeWeapon weapon && __result)
        {
            weapon.Invalidate();
        }
    }

    #endregion harmony patches
}
