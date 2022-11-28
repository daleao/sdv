namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatcher"/> class.</summary>
    internal GameLocationPerformActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Add Dark Sword transformation.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne || action is null || !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            if (action.StartsWith("Yoba") && who.CurrentTool is MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex } &&
                !who.mailReceived.Contains("gotHolyBlade"))
            {
                who.Halt();
                who.faceDirection(2);
                who.showCarrying();
                who.jitterStrength = 1f;
                Game1.pauseThenDoFunction(3000, Utils.GetHolyBlade);
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
                    Game1.afterDialogues,
                    (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));
            }
            else if (action.StartsWith("GoldenScythe"))
            {
                if (!Game1.player.mailReceived.Contains("gotGoldenScythe"))
                {
                    if (!Game1.player.isInventoryFull())
                    {
                        Game1.playSound("parry");
                        Game1.player.mailReceived.Add("gotGoldenScythe");
                        __instance.setMapTileIndex(29, 4, 245, "Front");
                        __instance.setMapTileIndex(30, 4, 246, "Front");
                        __instance.setMapTileIndex(29, 5, 261, "Front");
                        __instance.setMapTileIndex(30, 5, 262, "Front");
                        __instance.setMapTileIndex(29, 6, 277, "Buildings");
                        __instance.setMapTileIndex(30, 56, 278, "Buildings");
                        Game1.player.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(Constants.GoldenScytheIndex));
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    }
                }
                else if (!Game1.player.mailReceived.Contains("gotDarkSword") && !Game1.player.isInventoryFull())
                {
                    ProposeGrabDarkSword(__instance);
                }
                else
                {
                    Game1.changeMusicTrack("none");
                    __instance.performTouchAction("MagicWarp Mine 67 10", Game1.player.getStandingPosition());
                }
            }
            else
            {
                return true; // run original logic
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region injected subroutines

    private static void ProposeGrabDarkSword(GameLocation location)
    {
        Game1.multipleDialogues(new string[]
        {
            ModEntry.i18n.Get("darksword.found"),
            ModEntry.i18n.Get("darksword.chill"),
        });

        Game1.afterDialogues = () => location.createQuestionDialogue(
            ModEntry.i18n.Get("darksword.question"),
            new Response[]
            {
                new("GrabIt", ModEntry.i18n.Get("darksword.grabit")),
                new("LeaveIt", ModEntry.i18n.Get("darksword.leaveit")),
            },
            "DarkSword");
    }

    #endregion injected subroutines
}
