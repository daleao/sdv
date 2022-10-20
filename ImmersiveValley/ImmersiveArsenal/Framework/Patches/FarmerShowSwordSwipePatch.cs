namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Stardew.Arsenal.Framework.Events;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowSwordSwipePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowSwordSwipePatch"/> class.</summary>
    internal FarmerShowSwordSwipePatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showSwordSwipe));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowSwordSwipePrefix(Farmer who)
    {
        var sprite = who.FarmerSprite;
        if (sprite.currentAnimationIndex < 6 || ModEntry.State.ComboHitStep < ComboHitStep.FirstHit ||
            who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword } sword)
        {
            return true; // run original logic
        }

        TemporaryAnimatedSprite? tempSprite = null;
        var actionTile = who.GetToolLocation(ignoreClick: true);
        sword.DoDamage(who.currentLocation, (int)actionTile.X, (int)actionTile.Y, who.FacingDirection, 1, who);
        const int minSwipeInterval = 20;
        var index = sprite.currentAnimationIndex;
        switch (who.FacingDirection)
        {
            case Game1.up:
                switch (index)
                {
                    case 7 or 13 or 19:
                        who.yVelocity = 0.5f;
                        break;
                    case 11 or 17 or 23:
                        who.yVelocity = -0.3f;

                        var rotation = sprite.currentAnimationIndex is 11 or 23
                            ? (float)Math.PI * -5f / 4f
                            : (float)Math.PI * 5f / 4f;
                        tempSprite = new TemporaryAnimatedSprite(
                            "LooseSprites\\Cursors",
                            new Rectangle(518, 274, 23, 31),
                            who.Position + (new Vector2(0f, -32f) * 4f),
                            flipped: sprite.currentAnimationIndex is 11 or 23,
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
                    case 7 or 13 or 19:
                        who.xVelocity = 0.5f;
                        break;
                    case 11 or 17 or 23:
                        who.xVelocity = -0.3f;

                        var rotation = sprite.currentAnimationIndex is 11 or 23
                            ? (float)Math.PI
                            : 0f;
                        var offset = sprite.currentAnimationIndex is 11 or 23
                            ? new Vector2(12f, -32f) * 4f
                            : new Vector2(4f, -12f) * 4f;
                        tempSprite = new TemporaryAnimatedSprite(
                            "LooseSprites\\Cursors",
                            new Rectangle(518, 274, 23, 31),
                            who.Position + offset,
                            flipped: sprite.currentAnimationIndex is 11 or 23,
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
                    case 7 or 13 or 19:
                        who.yVelocity = -0.5f;
                        break;
                    case 11 or 17 or 23:
                        who.yVelocity = 0.3f;
                        tempSprite =
                            new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(503, 256, 42, 17),
                                who.Position + (new Vector2(-16f, -2f) * 4f),
                                flipped: sprite.currentAnimationIndex is 11 or 23,
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
                    case 7 or 13 or 19:
                        who.xVelocity = -0.5f;
                        break;
                    case 11 or 17 or 23:
                        who.xVelocity = 0.3f;

                        var rotation = sprite.currentAnimationIndex is 11 or 23
                            ? (float)Math.PI
                            : 0f;
                        var offset = sprite.currentAnimationIndex is 11 or 23
                            ? new Vector2(-18f, -28f) * 4f
                            : new Vector2(-15f, -12f) * 4f;
                        tempSprite = new TemporaryAnimatedSprite(
                            "LooseSprites\\Cursors",
                            new Rectangle(518, 274, 23, 31),
                            who.Position + offset,
                            flipped: sprite.currentAnimationIndex is not (11 or 23),
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

        if (sword.InitialParentTileIndex == 4)
        {
            tempSprite.color = Color.HotPink;
        }

        who.currentLocation.temporarySprites.Add(tempSprite);

        return false; // don't run original logic
    }

    [HarmonyPostfix]
    private static void FarmerShowSwordSwipePostfix(Farmer who)
    {
        Log.D($"Frame of farmer animation: {who.FarmerSprite.CurrentFrame}");

        if (who.FarmerSprite.currentAnimationIndex % 6 == 1)
        {
            ++ModEntry.State.ComboHitStep;
            Log.D($"Combo hit step: {ModEntry.State.ComboHitStep}");
        }
        else if (who.FarmerSprite.currentAnimationIndex > 6 && who.FarmerSprite.currentAnimationIndex % 6 == 2)
        {
            who.currentLocation.localSound("swordswipe");
        }
    }

    #endregion harmony patches
}
