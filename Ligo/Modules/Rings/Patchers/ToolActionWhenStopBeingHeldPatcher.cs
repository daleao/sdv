namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

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
        if (!ModEntry.Config.EnableArsenal)
        {
            return;
        }

        switch (__instance)
        {
            case MeleeWeapon weapon:
                weapon.RemoveResonances();
                break;
            case Slingshot slingshot:
                slingshot.RemoveResonances();
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}
