namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GreenSlimeDrawPrefix(
        NetBool ___avoidingMate,
        NetBool ___pursuingMate,
        NetVector2 ___facePosition,
        GreenSlime __instance,
        SpriteBatch b)
    {
        if (__instance.Get_Piped() is not { IsSummoned: true } piped)
        {
            return true; // run original logic
        }

        if (__instance.IsInvisible || !Utility.isOnScreen(__instance.Position, 128))
        {
            return false; // don't run original logic
        }

        var boundsHeight = __instance.GetBoundingBox().Height;
        var standingY = __instance.StandingPixel.Y;
        for (var i = 0; i <= __instance.stackedSlimes.Value; i++)
        {
            var topSlime = i == __instance.stackedSlimes.Value;
            var stackAdjustment = Vector2.Zero;
            if (__instance.stackedSlimes.Value > 0)
            {
                stackAdjustment =
                    new Vector2(
                        (float)Math.Sin(__instance.randomStackOffset +
                                        (Game1.currentGameTime.TotalGameTime.TotalSeconds * Math.PI * 2.0) + (i * 30)) *
                        8f,
                        -30 * i);
            }

            b.Draw(
                __instance.Sprite.Texture,
                __instance.getLocalPosition(Game1.viewport) +
                new Vector2(32f, (boundsHeight / 2) + __instance.yOffset) + stackAdjustment,
                __instance.Sprite.SourceRect,
                (__instance.prismatic.Value
                    ? Utility.GetPrismaticColor(348 + __instance.specialNumber.Value, 5f)
                    : __instance.color.Value) * piped.Alpha,
                0f,
                new Vector2(8f, 16f),
                4f * Math.Max(0.2f, __instance.Scale - (0.4f * (__instance.ageUntilFullGrown.Value / 120000f))),
                SpriteEffects.None,
                Math.Max(0f, __instance.drawOnTop ? 0.991f : (standingY + (i * 2)) / 10000f));
            b.Draw(
                Game1.shadowTexture,
                __instance.getLocalPosition(Game1.viewport) + new Vector2(
                    32f,
                    (boundsHeight / 2 * 7 / 4f) + __instance.yOffset + (8f * __instance.Scale) -
                    (__instance.ageUntilFullGrown.Value > 0 ? 8 : 0)) + stackAdjustment,
                Game1.shadowTexture.Bounds,
                Color.White * piped.Alpha,
                0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                3f + __instance.Scale - (__instance.ageUntilFullGrown.Value / 120000f) -
                ((__instance.Sprite.currentFrame % 4) % 3 != 0 || i != 0 ? 1f : 0f) + (__instance.yOffset / 30f),
                SpriteEffects.None,
                (standingY - 1 + (i * 2)) / 10000f);
            if (__instance.ageUntilFullGrown.Value <= 0)
            {
                if (topSlime && (__instance.cute.Value || __instance.hasSpecialItem.Value) && piped.Hat is null)
                {
                    var xDongleSource = __instance.isMoving() || __instance.wagTimer > 0
                        ? (16 * Math.Min(
                            7,
                            Math.Abs((__instance.wagTimer > 0
                                ? 992 - __instance.wagTimer
                                : Game1.currentGameTime.TotalGameTime.Milliseconds % 992) - 496) / 62)) % 64
                        : 48;
                    var yDongleSource = __instance.isMoving() || __instance.wagTimer > 0
                        ? 24 * Math.Min(
                            1,
                            Math.Max(
                                1,
                                Math.Abs((__instance.wagTimer > 0
                                    ? 992 - __instance.wagTimer
                                    : Game1.currentGameTime.TotalGameTime.Milliseconds % 992) - 496) / 62) / 4)
                        : 24;
                    if (__instance.hasSpecialItem.Value)
                    {
                        yDongleSource += 48;
                    }

                    b.Draw(
                        __instance.Sprite.Texture,
                        __instance.getLocalPosition(Game1.viewport) + stackAdjustment + (new Vector2(
                                32f,
                                boundsHeight - 16 + (__instance.readyToJump <= 0
                                    ? 4 * (-2 + Math.Abs((__instance.Sprite.currentFrame % 4) - 2))
                                    : 4 + (4 * ((__instance.Sprite.currentFrame % 4) % 3))) + __instance.yOffset) *
                            __instance.Scale),
                        new Rectangle(xDongleSource, 168 + yDongleSource, 16, 24),
                        (__instance.hasSpecialItem.Value ? Color.White : __instance.color.Value) * piped.Alpha,
                        0f,
                        new Vector2(8f, 16f),
                        4f * Math.Max(0.2f, __instance.Scale - (0.4f * (__instance.ageUntilFullGrown.Value / 120000f))),
                        __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        Math.Max(0f, __instance.drawOnTop ? 0.991f : (standingY / 10000f) + 0.0001f));
                }

                b.Draw(
                    __instance.Sprite.Texture,
                    __instance.getLocalPosition(Game1.viewport) + stackAdjustment + ((new Vector2(
                                32f,
                                (boundsHeight / 2) + (__instance.readyToJump <= 0
                                    ? 4 * (-2 + Math.Abs((__instance.Sprite.currentFrame % 4) - 2))
                                    : 4 - (4 * ((__instance.Sprite.currentFrame % 4) % 3))) + __instance.yOffset) +
                            ___facePosition.Value) *
                        Math.Max(0.2f, __instance.Scale - (0.4f * (__instance.ageUntilFullGrown.Value / 120000f)))),
                    new Rectangle(
                        32 + (__instance.readyToJump > 0 || __instance.focusedOnFarmers ? 16 : 0),
                        120 + (__instance.readyToJump < 0 &&
                               (__instance.focusedOnFarmers || __instance.invincibleCountdown > 0)
                            ? 24
                            : 0),
                        16,
                        24),
                    (Color.White * (__instance.FacingDirection == 0 ? 0.5f : 1f)) * piped.Alpha,
                    0f,
                    new Vector2(8f, 16f),
                    4f * Math.Max(0.2f, __instance.Scale - (0.4f * (__instance.ageUntilFullGrown.Value / 120000f))),
                    SpriteEffects.None,
                    Math.Max(0f, __instance.drawOnTop ? 0.991f : ((standingY + (i * 2)) / 10000f) + 0.0001f));
            }

            if (__instance.isGlowing)
            {
                b.Draw(
                    __instance.Sprite.Texture,
                    __instance.getLocalPosition(Game1.viewport) + stackAdjustment +
                    new Vector2(32f, (boundsHeight / 2) + __instance.yOffset),
                    __instance.Sprite.SourceRect,
                    (__instance.glowingColor * __instance.glowingTransparency) * piped.Alpha,
                    0f,
                    new Vector2(8f, 16f),
                    4f * Math.Max(0.2f, __instance.Scale),
                    SpriteEffects.None,
                    Math.Max(0f, __instance.drawOnTop ? 0.99f : (standingY / 10000f) + 0.001f));
            }
        }

        if (___pursuingMate.Value)
        {
            b.Draw(
                __instance.Sprite.Texture,
                __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, -32 + __instance.yOffset),
                new Rectangle(16, 120, 8, 8),
                Color.White * piped.Alpha,
                0f,
                new Vector2(3f, 3f),
                4f,
                SpriteEffects.None,
                Math.Max(0f, __instance.drawOnTop ? 0.991f : __instance.StandingPixel.Y / 10000f));
        }
        else if (___avoidingMate.Value)
        {
            b.Draw(
                __instance.Sprite.Texture,
                __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, -32 + __instance.yOffset),
                new Rectangle(24, 120, 8, 8),
                Color.White * piped.Alpha,
                0f,
                new Vector2(4f, 4f),
                4f,
                SpriteEffects.None,
                Math.Max(0f, __instance.drawOnTop ? 0.991f : __instance.StandingPixel.Y / 10000f));
        }

        if (piped.Hat is not null)
        {
            piped.DrawHat(b);
        }

        return false; // don't run original logic
    }

    /// <summary>Draw Piped Slime health.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeDrawPostfix(GreenSlime __instance, SpriteBatch b)
    {
        if (!Config.ShowMinionHealth || __instance.Get_Piped() is not { Hat: null })
        {
            return;
        }

        const float fullBarWidth = Game1.tileSize * 0.67f;
        var position = __instance.getLocalPosition(Game1.viewport);
        position.Y += __instance.Sprite.SpriteHeight * 2.5f;
        var fillPercent = (float)__instance.Health / __instance.MaxHealth;
        var width = fullBarWidth * fillPercent;
        position.X += (__instance.Sprite.SpriteWidth * 2) - (width / 2f) + 2;
        const int height = 4;
        var color = Utility.getRedToGreenLerpColor(fillPercent);
        b.Draw(Game1.staminaRect, new Rectangle((int)position.X, (int)position.Y, (int)width, height), color);
    }

    #endregion harmony patches
}
