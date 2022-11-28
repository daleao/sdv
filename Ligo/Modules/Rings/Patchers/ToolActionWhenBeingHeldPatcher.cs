namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenBeingHeldPatcher"/> class.</summary>
    internal ToolActionWhenBeingHeldPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset applied arsenal resonances.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenBeingHeldPostfix(Tool __instance)
    {
        if (!ModEntry.Config.EnableArsenal)
        {
            return;
        }

        switch (__instance)
        {
            case MeleeWeapon weapon:
                weapon.RecalculateResonances();
                break;
            case Slingshot slingshot:
                slingshot.RecalculateResonances();
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}
