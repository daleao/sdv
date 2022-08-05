namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationPerformActionPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Add Dark Sword transformation.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformTouchActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ModEntry.Config.InfinityPlusOneWeapons || action?.StartsWith("Yoba") != true || !who.IsLocalPlayer ||
            who.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DARK_SWORD_INDEX_I } darkSword ||
            darkSword.Read<int>("EnemiesSlain") < ModEntry.Config.RequiredKillCountToPurifyDarkSword ||
            who.mailReceived.Contains("holyBlade")) return true; // run original logic

        who.Halt();
        who.faceDirection(2);
        who.showCarrying();
        who.jitterStrength = 1f;
        Game1.pauseThenDoFunction(3000, Extensions.FarmerExtensions.GetHolyBlade);
        Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
        __instance.playSound("crit");
        Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
        DelayedAction.playSoundAfterDelay("stardrop", 1500);
        Game1.screenOverlayTempSprites.AddRange(
            Utility.sparkleWithinArea(new(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.Gold, 10,
                2000));
        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues,
            (Game1.afterFadeFunction)delegate { Game1.stopMusicTrack(Game1.MusicContext.Event); });

        return false; // don't run original logic
    }

    #endregion harmony patches
}