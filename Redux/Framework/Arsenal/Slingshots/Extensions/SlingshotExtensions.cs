// ReSharper disable PossibleLossOfFraction
namespace DaLion.Redux.Framework.Arsenal.Slingshots.Extensions;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Arsenal.Slingshots.Events;
using DaLion.Redux.Framework.Arsenal.Slingshots.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
internal static class SlingshotExtensions
{
    /// <summary>Analogous to <see cref="MeleeWeapon.animateSpecialMove"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    internal static void AnimateSpecialMove(this Slingshot slingshot)
    {
        if (Game1.fadeToBlack)
        {
            return;
        }

        var user = slingshot.getLastFarmerToUse();
        if (user is null || user.CurrentTool != slingshot)
        {
            return;
        }

        if (user.isEmoteAnimating)
        {
            user.EndEmoteAnimation();
        }

        slingshot.BeginSpecialMove(user);
        ModEntry.Events.Enable<SlingshotSpecialUpdateTickedEvent>();
    }

    /// <summary>Analogous to "MeleeWeapon.beginSpecialMove".</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    internal static void BeginSpecialMove(this Slingshot slingshot, Farmer who)
    {
        if (Game1.fadeToBlack)
        {
            return;
        }

        slingshot.Set_IsOnSpecial(true);
        who.UsingTool = true;
        who.CanMove = false;
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.DoDamage"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="x">The X pixel position of the <paramref name="slingshot"/>.</param>
    /// <param name="y">The Y pixel position of the <paramref name="slingshot"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    internal static void DoDamage(this Slingshot slingshot, int x, int y, Farmer who)
    {
        var areaOfEffect = slingshot.GetAreaOfEffect(who);
        Log.D(areaOfEffect.ToString());
        var damage = 10 * slingshot.InitialParentTileIndex switch
        {
            Constants.MasterSlingshotIndex => 2,
            Constants.GalaxySlingshotIndex => 4,
            _ => 1,
        };

        var location = who.currentLocation;
        if (location.damageMonster(
                areaOfEffect,
                damage - 2,
                damage + 3,
                false,
                0.5f,
                0,
                0,
                0,
                true,
                who))
        {
            location.playSound("clubhit");
        }

        who.UsingTool = false;
        who.CanMove = true;
        who.FarmerSprite.PauseForSingleAnimation = false;

        var dummyWeapon = new MeleeWeapon { BaseName = string.Empty };
        ModEntry.Reflector.GetUnboundFieldSetter<Tool, Farmer>(dummyWeapon, "lastUser").Invoke(dummyWeapon, who);
        var v = new Vector2(x / 64, y / 64);

        if (location.terrainFeatures.ContainsKey(v) &&
            location.terrainFeatures[v].performToolAction(dummyWeapon, 0, v, location))
        {
            location.terrainFeatures.Remove(v);
        }

        if (location.objects.ContainsKey(v) && location.objects[v].performToolAction(dummyWeapon, location))
        {
            location.objects.Remove(v);
        }

        location.performToolAction(dummyWeapon, (int)v.X, (int)v.Y);

        location.projectiles.Filter(delegate(Projectile projectile)
        {
            if (areaOfEffect.Intersects(projectile.getBoundingBox()) && !projectile.ignoreMeleeAttacks.Value)
            {
                projectile.behaviorOnCollisionWithOther(location);
            }

            return !projectile.destroyMe;
        });

        slingshot.CurrentParentTileIndex = slingshot.IndexOfMenuItemView;
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.getAreaOfEffect"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    /// <returns>A <see cref="Rectangle"/> representing the attack's area of effect.</returns>
    /// <remarks>Doesn't need to <see langword="switch"/> based on weapon type, so the <see cref="Slingshot"/> instance itself is unused.</remarks>
    internal static Rectangle GetAreaOfEffect(this Slingshot slingshot, Farmer who)
    {
        var (x, y) = who.getTileLocation() * Game1.tileSize;
        return (FacingDirection)who.FacingDirection switch
        {
            FacingDirection.Up => new Rectangle((int)x, (int)y - Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Right => new Rectangle((int)x + Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            FacingDirection.Down => new Rectangle((int)x, (int)y + Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Left => new Rectangle((int)x - Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Rectangle>(
                (FacingDirection)who.FacingDirection),
        };
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.drawDuringUse(int, int, SpriteBatch, Vector2, Farmer, Rectangle, int, bool)"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="frameOfFarmerAnimation">The frame of the <see cref="Farmer"/>'s current animation.</param>
    /// <param name="facingDirection">The <see cref="Farmer"/>'s facing direction.</param>
    /// <param name="b">The <see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="playerPosition">The <see cref="Farmer"/>'s position.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    /// <param name="sourceRect">The source <see cref="Rectangle"/> of the <paramref name="slingshot"/>'s texture.</param>
    /// <remarks>Doesn't need to <see langword="switch"/> based on weapon type, so the <see cref="Slingshot"/> instance itself is unused.</remarks>
    internal static void DrawDuringUse(
        this Slingshot slingshot,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch b,
        Vector2 playerPosition,
        Farmer who,
        Rectangle sourceRect)
    {
        switch (facingDirection)
        {
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 48f, playerPosition.Y - 104f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X, playerPosition.Y - 120f),
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f, playerPosition.Y - 108f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 2f / 6f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 115f, playerPosition.Y - 60f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >= 4:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 115f, playerPosition.Y - 54f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f, playerPosition.Y - 120f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X, playerPosition.Y - 120f),
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 66f, playerPosition.Y - 52f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -2f / 6f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 53f, playerPosition.Y + 4f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >= 4:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 53f, playerPosition.Y + 10f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            default:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 10f, playerPosition.Y - 114f),
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 1:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 2f, playerPosition.Y - 102f),
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 2:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 2f, playerPosition.Y - 86f),
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case >= 3:
                        if (facingDirection == Game1.down)
                        {
                            b.Draw(
                                Tool.weaponsTexture,
                                new Vector2(playerPosition.X + 61f, playerPosition.Y + 55f),
                                sourceRect,
                                Color.White,
                                (float)Math.PI,
                                Vector2.Zero,
                                4f,
                                SpriteEffects.None,
                                Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        }
                        else
                        {
                            b.Draw(
                                Tool.weaponsTexture,
                                new Vector2(playerPosition.X, playerPosition.Y - 68f),
                                sourceRect,
                                Color.White,
                                0f,
                                Vector2.Zero,
                                4f,
                                SpriteEffects.None,
                                Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        }

                        break;
                }

                if (facingDirection == Game1.up)
                {
                    who.FarmerRenderer.draw(
                        b,
                        who.FarmerSprite,
                        who.FarmerSprite.SourceRect,
                        who.getLocalPosition(Game1.viewport),
                        new Vector2(0f, ((who.yOffset + 128f - (who.GetBoundingBox().Height / 2)) / 4f) + 4f),
                        Math.Max(0f, (who.getStandingY() / 10000f) + 0.0099f),
                        Color.White,
                        0f,
                        who);
                }

                break;
        }
    }
}
