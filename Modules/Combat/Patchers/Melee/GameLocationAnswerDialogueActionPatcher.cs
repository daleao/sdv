namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatcher"/> class.</summary>
    internal GameLocationAnswerDialogueActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Convert the type of legendary sword.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(ref bool __result, string? questionAndAnswer)
    {
        if (!CombatModule.Config.EnableStabbingSwords || questionAndAnswer?.StartsWith("PillarsConvert") != true)
        {
            return true; // run original logic
        }

        if (questionAndAnswer.EndsWith("No"))
        {
            __result = false;
            return false; // don't run original logic
        }

        try
        {
            var player = Game1.player;
            if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.GalaxySword or WeaponIds.InfinityBlade } weapon)
            {
                return false; // don't run original logic
            }

            player.Halt();
            player.CanMove = false;
            player.faceDirection(Game1.down);

            var sprite = player.FarmerSprite;
            sprite.setCurrentFrame(128, 0, 2500, 1, false, false);
            EventManager.Enable<PutYourHandsUpUpdateTickedEvent>(); // this god damn animation doesn't stick unless we set it every frame
            Game1.currentLocation.temporarySprites.Add(
                new TemporaryAnimatedSprite(
                        "TileSheets\\weapons",
                        Game1.getSquareSourceRectForNonStandardTileSheet(
                            Tool.weaponsTexture,
                            16,
                            16,
                            weapon.IndexOfMenuItemView),
                        2500f,
                        1,
                        0,
                        player.Position + new Vector2(0f, -124f),
                        flicker: false,
                        flipped: false,
                        1f,
                        0f,
                        Color.White,
                        4f,
                        0f,
                        0f,
                        0f)
                    { motion = new Vector2(0f, -0.1f) });

            Game1.delayedActions.Add(new DelayedAction(2500, () =>
                Game1.currentLocation.temporarySprites.Add(
                    new TemporaryAnimatedSprite(
                        "TileSheets\\weapons",
                        Game1.getSquareSourceRectForNonStandardTileSheet(
                            Tool.weaponsTexture,
                            16,
                            16,
                            weapon.IndexOfMenuItemView),
                        500f,
                        1,
                        0,
                        player.Position + new Vector2(0f, -140f),
                        flicker: false,
                        flipped: false,
                        1f,
                        0f,
                        Color.White,
                        4f,
                        0f,
                        0f,
                        0f))));

            player.jitterStrength = 1f;
            Game1.pauseThenDoFunction(3000, () =>
            {
                var player = Game1.player;
                if (player.CurrentTool is not MeleeWeapon weapon)
                {
                    return;
                }

                Game1.flashAlpha = 1f;
                EventManager.Disable<PutYourHandsUpUpdateTickedEvent>();

                player.completelyStopAnimatingOrDoingAction();
                player.freezePause = 4000;
                player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[]
                {
                    new(57, 0), new(57, 2500, secondaryArm: false, flip: false, farmer =>
                    {
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                            "TileSheets\\weapons",
                            Game1.getSquareSourceRectForNonStandardTileSheet(
                                Tool.weaponsTexture,
                                16,
                                16,
                                weapon.IndexOfMenuItemView),
                            2500f,
                            1,
                            0,
                            farmer.Position + new Vector2(0f, -140f),
                            flicker: false,
                            flipped: false,
                            1f,
                            0f,
                            Color.White,
                            4f,
                            0f,
                            0f,
                            0f) { motion = new Vector2(0f, +0.1f) });
                    }),
                    new(
                        (short)player.FarmerSprite.CurrentFrame,
                        500,
                        secondaryArm: false,
                        flip: false,
                        null,
                        behaviorAtEndOfFrame: true),
                });

                weapon.type.Value = weapon.type.Value switch
                {
                    MeleeWeapon.stabbingSword => MeleeWeapon.defenseSword,
                    MeleeWeapon.defenseSword => MeleeWeapon.stabbingSword,
                    _ => weapon.type.Value,
                };

                weapon.Write(DataKeys.SwordType, weapon.type.Value.ToString());
                Game1.player.jitterStrength = 0f;
                Game1.screenGlowHold = false;
            });

            Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
            Game1.currentLocation.playSound("crit");
            Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
            DelayedAction.playSoundAfterDelay("stardrop", 500);
            Game1.screenOverlayTempSprites.AddRange(
                Utility.sparkleWithinArea(
                    new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                    500,
                    Color.Gold,
                    10,
                    1000));
            Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(
                Game1.afterDialogues, (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));
            __result = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
