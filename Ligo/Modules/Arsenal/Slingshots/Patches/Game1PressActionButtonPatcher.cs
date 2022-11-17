namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Slingshots.Extensions;
using DaLion.Ligo.Modules.Arsenal.Slingshots.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class Game1PressActionButtonPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1PressActionButtonPatcher"/> class.</summary>
    internal Game1PressActionButtonPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.pressActionButton));
    }

    #region harmony patches

    /// <summary>Triggers slingshot special move.</summary>
    [HarmonyPostfix]
    private static void Game1PressActionButtonPostfix(ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not Slingshot slingshot || slingshot.Get_IsOnSpecial() || player.usingSlingshot ||
            !player.CanMove || player.canOnlyWalk || player.onBridge.Value || !Game1.didPlayerJustRightClick(true))
        {
            return;
        }

        slingshot.AnimateSpecialMove();
        __result = false;
    }

    #endregion harmony patches
}
