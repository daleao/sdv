namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
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

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenStopBeingHeldPostfix(Tool __instance, Farmer who)
    {
        if (__instance is not MeleeWeapon || !who.IsLocalPlayer)
        {
            return;
        }

        CombatModule.State.ComboHitQueued = ComboHitStep.Idle;
        CombatModule.State.ComboHitStep = ComboHitStep.Idle;
    }

    #endregion harmony patches
}
