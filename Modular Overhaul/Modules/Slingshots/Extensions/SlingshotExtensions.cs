namespace DaLion.Overhaul.Modules.Slingshots.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Overhaul.Modules.Slingshots.Events;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Exceptions;
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
        if (slingshot.hasEnchantmentOfType<RangedArtfulEnchantment>())
        {
            EventManager.Enable<SlingshotArtfulSpecialUpdateTickedEvent>();
        }
        else
        {
            EventManager.Enable<SlingshotSpecialUpdateTickedEvent>();
        }
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
        var areaOfEffect = slingshot.GetAreaOfEffect(x, y, who);
        var damage = 10 * slingshot.InitialParentTileIndex switch
        {
            ItemIDs.MasterSlingshot => 2,
            ItemIDs.GalaxySlingshot => 3,
            ItemIDs.InfinitySlingshot => 4,
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
        Reflector.GetUnboundFieldSetter<Tool, Farmer>(dummyWeapon, "lastUser").Invoke(dummyWeapon, who);
        var v = new Vector2(x / Game1.tileSize, y / Game1.tileSize);

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
    /// <param name="x">The target x-coordinate.</param>
    /// <param name="y">The target y-coordinate.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    /// <returns>A <see cref="Rectangle"/> representing the attack's area of effect.</returns>
    /// <remarks>Doesn't need to <see langword="switch"/> based on weapon type, so the <see cref="Slingshot"/> instance itself is unused.</remarks>
    internal static Rectangle GetAreaOfEffect(this Slingshot slingshot, int x, int y, Farmer who)
    {
        var areaOfEffect = Rectangle.Empty;
        if (!slingshot.hasEnchantmentOfType<RangedArtfulEnchantment>())
        {
            areaOfEffect = who.FacingDirection switch
            {
                Game1.up => new Rectangle(x, y - Game1.tileSize, Game1.tileSize, Game1.tileSize),
                Game1.right => new Rectangle(x + Game1.tileSize, y, Game1.tileSize, Game1.tileSize),
                Game1.down => new Rectangle(x, y + Game1.tileSize, Game1.tileSize, Game1.tileSize),
                Game1.left => new Rectangle(x - Game1.tileSize, y, Game1.tileSize, Game1.tileSize),
                _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Rectangle>(
                    (FacingDirection)who.FacingDirection),
            };
            areaOfEffect.Inflate(16, 16);
            return areaOfEffect;
        }

        const int width = 64;
        const int height = 64;
        const int horizontalYOffset = -32;
        const int upHeightOffset = 0;
        var facingDirection = who.FacingDirection;
        var wielderBoundingBox = who.GetBoundingBox();
        var indexInCurrentAnimation = who.FarmerSprite.currentAnimationIndex;
        switch (facingDirection)
        {
            case Game1.up:
                areaOfEffect = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Y - height - upHeightOffset,
                    width,
                    height + upHeightOffset);
                switch (indexInCurrentAnimation)
                {
                    case 5:
                        areaOfEffect.Offset(76, -32);
                        break;
                    case 4:
                        areaOfEffect.Offset(56, -32);
                        areaOfEffect.Height += 32;
                        break;
                    case 3:
                        areaOfEffect.Offset(40, -60);
                        areaOfEffect.Height += 48;
                        break;
                    case 2:
                        areaOfEffect.Offset(-12, -68);
                        areaOfEffect.Height += 48;
                        break;
                    case 1:
                        areaOfEffect.Offset(-48, -56);
                        areaOfEffect.Height += 32;
                        break;
                    case 0:
                        areaOfEffect.Offset(-60, -12);
                        break;
                }

                break;
            case Game1.right:
                areaOfEffect = new Rectangle(
                    wielderBoundingBox.Right,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                switch (indexInCurrentAnimation)
                {
                    case 0:
                        areaOfEffect.Offset(-44, -84);
                        break;
                    case 1:
                        areaOfEffect.Offset(4, -44);
                        break;
                    case 2:
                        areaOfEffect.Offset(12, -4);
                        break;
                    case 3:
                        areaOfEffect.Offset(12, 37);
                        break;
                    case 4:
                        areaOfEffect.Offset(-28, 60);
                        break;
                    case 5:
                        areaOfEffect.Offset(-60, 72);
                        break;
                }

                break;
            case Game1.down:
                areaOfEffect = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Bottom,
                    width,
                    height);
                switch (indexInCurrentAnimation)
                {
                    case 0:
                        areaOfEffect.Offset(72, -92);
                        break;
                    case 1:
                        areaOfEffect.Offset(56, -32);
                        break;
                    case 2:
                        areaOfEffect.Offset(40, -28);
                        break;
                    case 3:
                        areaOfEffect.Offset(-12, -8);
                        break;
                    case 4:
                        areaOfEffect.Offset(-80, -24);
                        areaOfEffect.Width += 32;
                        break;
                    case 5:
                        areaOfEffect.Offset(-68, -44);
                        break;
                }

                break;
            case Game1.left:
                areaOfEffect = new Rectangle(
                    wielderBoundingBox.Left - height,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                switch (indexInCurrentAnimation)
                {
                    case 0:
                        areaOfEffect.Offset(56, -76);
                        break;
                    case 1:
                        areaOfEffect.Offset(-8, -56);
                        break;
                    case 2:
                        areaOfEffect.Offset(-16, -4);
                        break;
                    case 3:
                        areaOfEffect.Offset(0, 37);
                        break;
                    case 4:
                        areaOfEffect.Offset(24, 60);
                        break;
                    case 5:
                        areaOfEffect.Offset(64, 64);
                        break;
                }

                break;
        }

        return areaOfEffect;
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.drawDuringUse(int, int, SpriteBatch, Vector2, Farmer, Rectangle, int, bool)"/>, for overhead swipes (smash).</summary>
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
        Vector2 toolPosition;
        switch (facingDirection)
        {
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                    case 1:
                        toolPosition = new Vector2(playerPosition.X - 42f, playerPosition.Y - 99f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X + 34f, playerPosition.Y - 124f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        toolPosition = new Vector2(playerPosition.X + 78f, playerPosition.Y - 114f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 4:
                        toolPosition = new Vector2(playerPosition.X + 116f, playerPosition.Y - 61f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >= 5:
                        toolPosition = new Vector2(playerPosition.X + 114f, playerPosition.Y - 53f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
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
                    case 1:
                        toolPosition = new Vector2(playerPosition.X + 48f, playerPosition.Y - 122f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X - 28f, playerPosition.Y - 108f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 3:
                        toolPosition = new Vector2(playerPosition.X - 64f, playerPosition.Y - 68f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 4:
                        toolPosition = new Vector2(playerPosition.X - 54f, playerPosition.Y + 2f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 2f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >= 5:
                        toolPosition = new Vector2(playerPosition.X - 52f, playerPosition.Y + 11f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
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
                    case 1:
                        toolPosition = new Vector2(playerPosition.X - 20f, playerPosition.Y - 118f);
                        if (facingDirection == Game1.up)
                        {
                            toolPosition.X += 16f;
                        }

                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X - 2f, playerPosition.Y - 86f);
                        if (facingDirection == Game1.up)
                        {
                            toolPosition.X += 16f;
                        }

                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
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
                            toolPosition = new Vector2(playerPosition.X + 61f, playerPosition.Y + 55f);
                            b.Draw(
                                Tool.weaponsTexture,
                                toolPosition,
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
                            toolPosition = new Vector2(playerPosition.X, playerPosition.Y - 68f);
                            b.Draw(
                                Tool.weaponsTexture,
                                toolPosition,
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

    /// <summary>Analogous to <see cref="MeleeWeapon.drawDuringUse(int, int, SpriteBatch, Vector2, Farmer, Rectangle, int, bool)"/>, for horizontal swipes.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="frameOfFarmerAnimation">The frame of the <see cref="Farmer"/>'s current animation.</param>
    /// <param name="facingDirection">The <see cref="Farmer"/>'s facing direction.</param>
    /// <param name="b">The <see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="playerPosition">The <see cref="Farmer"/>'s position.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    /// <param name="sourceRect">The source <see cref="Rectangle"/> of the <paramref name="slingshot"/>'s texture.</param>
    /// <remarks>Doesn't need to <see langword="switch"/> based on weapon type, so the <see cref="Slingshot"/> instance itself is unused.</remarks>
    internal static void DrawDuringArtfulUse(
        this Slingshot slingshot,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch b,
        Vector2 playerPosition,
        Farmer who,
        Rectangle sourceRect)
    {
        Vector2 toolPosition, center;
        switch (facingDirection)
        {
            case Game1.up:
                center = new(1f, 15f);
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                    case 1:
                        toolPosition = new Vector2(playerPosition.X + 22f, playerPosition.Y - 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X + 2f, playerPosition.Y - 38f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI * 1f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 3:
                        toolPosition = new Vector2(playerPosition.X + 16f, playerPosition.Y - 52f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI * 1f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 4:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 48f - 10f - 12f, playerPosition.Y - 52f - 18f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 1f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 5:
                        toolPosition = new Vector2(playerPosition.X + 41f, playerPosition.Y - 63f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 1f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case >=6:
                        toolPosition = new Vector2(playerPosition.X + 56f, playerPosition.Y - 68f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 32 - 8) / 10000f));
                        break;
                }

                break;
            case Game1.right:
                center = new(8f, 15f);
                switch (frameOfFarmerAnimation)
                {
                    case 0: // done
                    case 1:
                        toolPosition = new Vector2(playerPosition.X + 40f, playerPosition.Y - 56f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X + 56f, playerPosition.Y - 36f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 3:
                        toolPosition = new Vector2(playerPosition.X + 60f, playerPosition.Y - 16f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 4:
                        toolPosition = new Vector2(playerPosition.X + 60f, playerPosition.Y - 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 5:
                        toolPosition = new Vector2(playerPosition.X + 36f, playerPosition.Y + 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 7f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >=6:
                        toolPosition = new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
            case Game1.down:
                center = new(1f, 15f);
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                    case 1:
                        toolPosition = new Vector2(playerPosition.X + 48f, playerPosition.Y - 44f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X + 74f, playerPosition.Y - 24f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 3:
                        toolPosition = new Vector2(playerPosition.X + 66f, playerPosition.Y - 14f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 4:
                        toolPosition = new Vector2(playerPosition.X + 44f, playerPosition.Y + 8f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case 5:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 16f, playerPosition.Y + 30f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 5f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                    case >=6:
                        toolPosition = new Vector2(playerPosition.X + 18f, playerPosition.Y + 26f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 11f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (who.getStandingY() + 32) / 10000f));
                        break;
                }

                break;
            case Game1.left:
                center = new(0f, 6f);
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                    case 1:
                        toolPosition = new Vector2(playerPosition.X - 16f + 4f, playerPosition.Y - 80f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 2:
                        toolPosition = new Vector2(playerPosition.X - 48f + 8f, playerPosition.Y - 44f + 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 3: // done
                        toolPosition = new Vector2(playerPosition.X - 32f, playerPosition.Y + 16f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() - 1) / 10000f));
                        break;
                    case 4:
                        toolPosition = new Vector2(playerPosition.X + 4f - 4f, playerPosition.Y + 44f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case 5:
                        toolPosition = new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f - 4f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI * 7f / 8f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                    case >=6: // done
                        toolPosition = new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f);
                        b.Draw(
                            Tool.weaponsTexture,
                            toolPosition,
                            sourceRect,
                            Color.White,
                            -(float)Math.PI,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (who.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }
    }

    /// <summary>Analogous to <see cref="Farmer.showSwordSwipe"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    internal static void ShowSwordSwipe(this Slingshot slingshot, Farmer who)
    {
        var facingDirection = who.FacingDirection;
        var tempSprite = facingDirection switch
        {
            Game1.up => new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors",
                new Rectangle(518, 274, 23, 31),
                who.Position + (new Vector2(0f, -30f) * 4f),
                flipped: false,
                0.07f,
                Color.White)
            {
                scale = 4f,
                animationLength = 1,
                interval = Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, 20),
                alpha = 0.5f,
                rotation = 3.926991f,
            },
            Game1.right => new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors",
                new Rectangle(518, 274, 23, 31),
                who.Position + (new Vector2(6f, -16f) * 4f),
                flipped: false,
                0.07f,
                Color.White)
            {
                scale = 4f,
                animationLength = 1,
                interval = Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, 20),
                alpha = 0.5f,
            },
            Game1.down => new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors",
                new Rectangle(503, 256, 42, 17),
                who.Position + (new Vector2(-10f, -4f) * 4f),
                flipped: false,
                0.07f,
                Color.White)
            {
                scale = 4f,
                animationLength = 1,
                interval = Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, 20),
                alpha = 0.5f,
                layerDepth = (who.Position.Y + 64f) / 10000f,
            },
            Game1.left => new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors",
                new Rectangle(518, 274, 23, 31),
                who.Position + (new Vector2(-15f, -16f) * 4f),
                flipped: false,
                0.07f,
                Color.White)
            {
                scale = 4f,
                animationLength = 1,
                interval = Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, 20),
                flipped = true,
                alpha = 0.5f,
            },
            _ => null,
        };

        if (tempSprite is not null)
        {
            who.currentLocation.temporarySprites.Add(tempSprite);
        }
    }
}
