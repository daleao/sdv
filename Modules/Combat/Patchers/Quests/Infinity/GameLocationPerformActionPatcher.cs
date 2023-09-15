namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
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

    /// <summary>Add Dark Sword + transformation.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!CombatModule.Config.EnableHeroQuest || action is null || !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            if (action.StartsWith("Yoba"))
            {
                HandleYobaAltar(__instance, who);
            }
            else if (action.StartsWith("GoldenScythe"))
            {
                HandleReaperStatue(__instance, who);
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

    #region handlers

    private static void HandleYobaAltar(GameLocation location, Farmer who)
    {
        if (!who.mailReceived.Contains("gotHolyBlade") && who.hasQuest((int)QuestId.HeroReward) &&
            who.CurrentTool is MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword })
        {
            who.Halt();
            who.CanMove = false;
            who.faceDirection(2);
            who.showCarrying();
            who.jitterStrength = 1f;
            Game1.pauseThenDoFunction(3000, GetHolyBlade);
            Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
            location.playSound("crit");
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
            who.completeQuest((int)QuestId.HeroReward);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Locations_SeedShop_Yoba());
            if (who.hasQuest((int)QuestId.CurseIntro))
            {
                Game1.afterDialogues = () =>
                {
                    var question = I18n.Yoba_Inscriptions();
                    var responses = Virtue.List
                        .Select(v => new Response(v.Name, v.DisplayName))
                        .ToArray();
                    location.createQuestionDialogue(question, responses, "Yoba");
                };
            }
            else if (CombatModule.State.HeroQuest is not null &&
                     who.CurrentTool is MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword } &&
                     !CombatModule.State.DidPrayToday)
            {
                Game1.afterDialogues = () =>
                {
                    var question = I18n.Yoba_Prayer();
                    location.createQuestionDialogue(question, location.createYesNoResponses(), "Yoba");
                };
            }
        }
    }

    private static void HandleReaperStatue(GameLocation location, Farmer who)
    {
        if (!who.mailReceived.Contains("gotGoldenScythe"))
        {
            if (!who.isInventoryFull())
            {
                Game1.playSound("parry");
                who.mailReceived.Add("gotGoldenScythe");
                location.setMapTileIndex(29, 4, 245, "Front");
                location.setMapTileIndex(30, 4, 246, "Front");
                location.setMapTileIndex(29, 5, 261, "Front");
                location.setMapTileIndex(30, 5, 262, "Front");
                location.setMapTileIndex(29, 6, 277, "Buildings");
                location.setMapTileIndex(30, 56, 278, "Buildings");
                who.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(WeaponIds.GoldenScythe));

                Game1.afterDialogues = () =>
                {
                   ProposeGrabDarkSword(location);
                };
            }
            else
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
        }
        else if (!who.mailReceived.Contains("gotDarkSword") && !who.isInventoryFull())
        {
            ProposeGrabDarkSword(location);
        }
        else
        {
            Game1.changeMusicTrack("none");
            location.performTouchAction("MagicWarp Mine 67 10", who.getStandingPosition());
        }
    }

    private static void ProposeGrabDarkSword(GameLocation location)
    {
        Game1.multipleDialogues(new[]
        {
            I18n.Weapons_DarkSword_Found(),
            I18n.Weapons_DarkSword_Chill(),
        });

        Game1.afterDialogues = () => location.createQuestionDialogue(
            I18n.Weapons_DarkSword_Question(),
            new Response[]
            {
                new("GrabIt", I18n.Weapons_DarkSword_GrabIt()),
                new("LeaveIt", I18n.Weapons_DarkSword_LeaveIt()),
            },
            "DarkSword");
    }

    private static void GetHolyBlade()
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword } darkSword)
        {
            Log.W($"[CMBT]: {player.Name} cannot receive the Holy Blade because they are not holding the Dark Sword!");
            return;
        }

        Game1.flashAlpha = 1f;
        player.holdUpItemThenMessage(new MeleeWeapon(WeaponIds.HolyBlade));
        darkSword.transform(WeaponIds.HolyBlade);
        darkSword.Write(DataKeys.CursePoints, null);
        darkSword.RefreshStats();
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
        player.mailReceived.Add("gotHolyBlade");
        EventManager.Disable<CurseUpdateTickedEvent>();
    }

    #endregion handlers
}
