// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using Common;
using Common.Enums;
using Common.Exceptions;
using Common.Extensions.Reflection;
using Framework.Events;
using Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
public static class SlingshotExtensions
{
    private static readonly Lazy<Action<Tool, Farmer>> _SetLastUser = new(() =>
        typeof(Tool).RequireField("lastUser").CompileUnboundFieldSetterDelegate<Tool, Farmer>());

    /// <summary>Analogous to <see cref="MeleeWeapon.animateSpecialMove"/>.</summary>
    public static void AnimateSpecialMove(this Slingshot slingshot)
    {
        if (Game1.fadeToBlack || ModEntry.State.SlingshotCooldown > 0) return;

        var user = slingshot.getLastFarmerToUse();
        if (user is null || user.CurrentTool != slingshot) return;

        if (user.isEmoteAnimating) user.EndEmoteAnimation();

        slingshot.BeginSpecialMove(user);
        ModEntry.Events.Enable<SlingshotSpecialUpdateTickedEvent>();
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.beginSpecialMove"/>.</summary>
    public static void BeginSpecialMove(this Slingshot slingshot, Farmer who)
    {
        if (Game1.fadeToBlack) return;

        slingshot.set_IsOnSpecial(true);
        who.UsingTool = true;
        who.CanMove = false;
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.DoDamage"/>.</summary>
    public static void DoDamage(this Slingshot slingshot, int x, int y, Farmer who)
    {
        var areaOfEffect = slingshot.GetAreaOfEffect(who);
        Log.D(areaOfEffect.ToString());
        var damage = 10 * slingshot.InitialParentTileIndex switch
        {
            Constants.MASTER_SLINGSHOT_INDEX_I => 2,
            Constants.GALAXY_SLINGSHOT_INDEX_I => 4,
            _ => 1
        };

        var location = who.currentLocation;
        if (location.damageMonster(areaOfEffect, damage - 2, damage + 3, false, 0.5f, 0, 0, 0, true,
                who)) location.playSound("clubhit");

        who.UsingTool = false;
        who.CanMove = true;
        who.FarmerSprite.PauseForSingleAnimation = false;

        var dummyWeapon = new MeleeWeapon { BaseName = string.Empty };
        _SetLastUser.Value(dummyWeapon, who);
        var v = new Vector2(x / 64, y / 64);

        if (location.terrainFeatures.ContainsKey(v) &&
            location.terrainFeatures[v].performToolAction(dummyWeapon, 0, v, location))
            location.terrainFeatures.Remove(v);

        if (location.objects.ContainsKey(v) && location.objects[v].performToolAction(dummyWeapon, location))
            location.objects.Remove(v);

        location.performToolAction(dummyWeapon, (int)v.X, (int)v.Y);

        location.projectiles.Filter(delegate (Projectile projectile)
        {
            if (areaOfEffect.Intersects(projectile.getBoundingBox()) && !projectile.ignoreMeleeAttacks.Value)
                projectile.behaviorOnCollisionWithOther(location);

            return !projectile.destroyMe;
        });

        slingshot.CurrentParentTileIndex = slingshot.IndexOfMenuItemView;
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.getAreaOfEffect"/>.</summary>
    public static Rectangle GetAreaOfEffect(this Slingshot slingshot, Farmer who)
    {
        var (x, y) = who.getTileLocation() * Game1.tileSize;
        return (FacingDirection)who.FacingDirection switch
        {
            FacingDirection.Up => new((int)x, (int)y - Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Right => new((int)x + Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            FacingDirection.Down => new((int)x, (int)y + Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Left => new((int)x - Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Rectangle>((FacingDirection)who.FacingDirection)
        };
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.drawDuringUse"/>.</summary>
    public static void DrawDuringUse(this Slingshot slingshot, int frameOfFarmerAnimation, int facingDirection,
        SpriteBatch b, Vector2 playerPosition, Farmer who, Rectangle sourceRect)
    {
        switch (facingDirection)
        {
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 48f, playerPosition.Y - 104f),
                            sourceRect, Color.White, -(float)Math.PI / 8f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X, playerPosition.Y - 120f),
                            sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X + 96f, playerPosition.Y - 108f), sourceRect,
                            Color.White, (float)Math.PI * 2f / 6f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        b.Draw(Tool.weaponsTexture,
                            new(playerPosition.X + 115f, playerPosition.Y - 60f), sourceRect,
                            Color.White, (float)Math.PI / 2f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >=4:
                        b.Draw(Tool.weaponsTexture,
                            new(playerPosition.X + 115f, playerPosition.Y - 54f), sourceRect,
                            Color.White, (float)Math.PI / 2f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X + 96f, playerPosition.Y - 120f), sourceRect,
                            Color.White, (float)Math.PI * 3f / 8f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X, playerPosition.Y - 120f), sourceRect,
                            Color.White, 0f, Vector2.Zero, 4f,
                            SpriteEffects.None, Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 66f, playerPosition.Y - 52f), sourceRect,
                            Color.White, (float)Math.PI * -2f / 6f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 53f, playerPosition.Y + 4f),
                            sourceRect, Color.White, -(float)Math.PI / 2f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >=4:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 53f, playerPosition.Y + 10f),
                            sourceRect, Color.White, -(float)Math.PI / 2f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
            default:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 10f, playerPosition.Y - 114f),
                            sourceRect, Color.White, 0f, Vector2.Zero, 4f,
                            SpriteEffects.None, Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 1:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 2f, playerPosition.Y - 102f), sourceRect,
                            Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 2:
                        b.Draw(Tool.weaponsTexture, new(playerPosition.X - 2f, playerPosition.Y - 86f), sourceRect,
                            Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case >=3:
                        if (facingDirection == Game1.down)
                            b.Draw(Tool.weaponsTexture, new(playerPosition.X + 61f, playerPosition.Y + 55f), sourceRect,
                                Color.White, (float)Math.PI, Vector2.Zero, 4f, SpriteEffects.None,
                                Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        else
                            b.Draw(Tool.weaponsTexture, new(playerPosition.X, playerPosition.Y - 68f), sourceRect,
                                Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                                Math.Max(0f, (who.getStandingY() + 32) / 10000f));

                        break;
                }

                if (facingDirection == Game1.up)
                {
                    who.FarmerRenderer.draw(b, who.FarmerSprite, who.FarmerSprite.SourceRect,
                        who.getLocalPosition(Game1.viewport),
                        new(0f, (who.yOffset + 128f - who.GetBoundingBox().Height / 2) / 4f + 4f),
                        Math.Max(0f, who.getStandingY() / 10000f + 0.0099f), Color.White, 0f, who);
                }

                break;
        }
    }
}