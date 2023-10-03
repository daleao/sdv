namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1PressActionButtonPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1PressActionButtonPatcher"/> class.</summary>
    internal Game1PressActionButtonPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.pressActionButton));
    }

    #region harmony patches

    /// <summary>Trigger slingshot special move.</summary>
    [HarmonyPostfix]
    private static void Game1PressActionButtonPostfix(ref bool __result)
    {
        if (!__result || !CombatModule.Config.EnableSlingshotSpecialMove || CombatModule.State.SlingshotCooldown > 0)
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not Slingshot slingshot || slingshot.Get_IsOnSpecial() || player.usingSlingshot ||
            !player.CanMove || player.canOnlyWalk || player.onBridge.Value || !Game1.didPlayerJustRightClick(true))
        {
            return;
        }

        if (ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is not null)
        {
            return;
        }

        slingshot.AnimateSpecialMove();
        __result = false;
    }

    #endregion harmony patches
}
