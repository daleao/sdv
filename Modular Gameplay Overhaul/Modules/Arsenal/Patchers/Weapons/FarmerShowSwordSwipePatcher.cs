namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowSwordSwipePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowSwordSwipePatcher"/> class.</summary>
    internal FarmerShowSwordSwipePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showSwordSwipe));
    }

    #region harmony patches

    /// <summary>Show combo swipe.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowSwordSwipePrefix(Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon || !ArsenalModule.Config.Weapons.EnableComboHits)
        {
            return true; // run original logic
        }

        try
        {
            const int minSwipeInterval = 20;
            var actionTile = who.GetToolLocation(ignoreClick: true);
            var sprite = who.FarmerSprite;
            var index = sprite.currentAnimationIndex;

            TemporaryAnimatedSprite? tempSprite = null;
            weapon.DoDamage(who.currentLocation, (int)actionTile.X, (int)actionTile.Y, who.FacingDirection, 1, who);
            var isBackwardsSwipe = (int)ArsenalModule.State.ComboHitStep % 2 == 0;
            switch (who.FacingDirection)
            {
                case Game1.up:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.yVelocity = 0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.yVelocity = -0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI * -5f / 4f
                                : (float)Math.PI * 5f / 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + (new Vector2(0f, -32f) * 4f),
                                flipped: flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;

                case Game1.right:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.xVelocity = 0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.xVelocity = -0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI
                                : 0f;
                            var offset = flipped
                                ? new Vector2(12f, -32f) * 4f
                                : new Vector2(4f, -12f) * 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + offset,
                                flipped: flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;

                case Game1.down:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.yVelocity = -0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.yVelocity = 0.3f;
                            tempSprite =
                                new TemporaryAnimatedSprite(
                                    "LooseSprites\\Cursors",
                                    new Rectangle(503, 256, 42, 17),
                                    who.Position + (new Vector2(-16f, -2f) * 4f),
                                    flipped: sprite.currentAnimationIndex is 11 or 23 || (sprite.currentAnimationIndex == 5 && isBackwardsSwipe),
                                    0.07f,
                                    Color.White)
                                {
                                    scale = 4f,
                                    animationLength = 1,
                                    interval = Math.Max(
                                        who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                        minSwipeInterval),
                                    alpha = 0.5f,
                                    layerDepth = (who.Position.Y + 64f) / 10000f,
                                };
                            break;
                    }

                    break;

                case Game1.left:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.xVelocity = -0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.xVelocity = 0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI
                                : 0f;
                            var offset = flipped
                                ? new Vector2(-18f, -28f) * 4f
                                : new Vector2(-15f, -12f) * 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + offset,
                                flipped: !flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;
            }

            if (tempSprite is null)
            {
                return false; // don't run original logic
            }

            tempSprite.color = weapon.IsInfinityWeapon()
                ? Color.HotPink
                : weapon.InitialParentTileIndex switch
                {
                    Constants.DarkSwordIndex => Color.DarkSlateGray,
                    Constants.HolyBladeIndex => Color.Gold,
                    _ => tempSprite.color,
                };

            who.currentLocation.temporarySprites.Add(tempSprite);

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
