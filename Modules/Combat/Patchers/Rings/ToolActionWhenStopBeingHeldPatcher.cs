namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenStopBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenStopBeingHeldPatcher"/> class.</summary>
    internal ToolActionWhenStopBeingHeldPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenStopBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset applied arsenal resonances.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenStopBeingHeldPostfix(Tool __instance)
    {
        __instance.UnsetAllResonatingChords();
    }

    #endregion harmony patches
}
