namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Common.Attributes;
using Extensions;
using HarmonyLib;
using StardewValley.Tools;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, Deprecated]
internal sealed class Game1PressActionButtonPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1PressActionButtonPatch()
    {
        Target = RequireMethod<Game1>(nameof(Game1.pressActionButton));
    }

    #region harmony patches

    /// <summary>Triggers slingshot special move.</summary>
    [HarmonyPostfix]
    private static void Game1PressActionButtonPostfix(ref bool __result)
    {
        if (!__result || !ModEntry.Config.EnableSlingshotSpecialMove) return;

        var player = Game1.player;
        if (player.CurrentTool is not Slingshot slingshot || slingshot.get_IsOnSpecial() || player.usingSlingshot ||
            !player.CanMove || player.canOnlyWalk || Game1.eventUp || player.onBridge.Value ||
            !Game1.didPlayerJustRightClick(true)) return;

        slingshot.AnimateSpecialMove();
        __result = false;
    }

    #endregion harmony patches
}