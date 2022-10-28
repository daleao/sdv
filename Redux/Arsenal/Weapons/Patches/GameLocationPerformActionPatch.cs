namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class GameLocationPerformActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatch"/> class.</summary>
    internal GameLocationPerformActionPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Add Dark Sword transformation.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformTouchActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ModEntry.Config.Arsenal.Weapons.InfinityPlusOneWeapons || action?.StartsWith("Yoba") != true ||
            !who.IsLocalPlayer ||
            who.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex } darkSword ||
            who.mailReceived.Contains("holyBlade"))
        {
            return true; // run original logic
        }

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
            Utility.sparkleWithinArea(
                new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                500,
                Color.Gold,
                10,
                2000));
        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(
            Game1.afterDialogues, (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));

        return false; // don't run original logic
    }

    #endregion harmony patches
}
