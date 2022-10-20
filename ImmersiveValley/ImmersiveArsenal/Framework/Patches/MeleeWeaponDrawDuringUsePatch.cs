namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawDuringUsePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawDuringUsePatch"/> class.</summary>
    internal MeleeWeaponDrawDuringUsePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(
            nameof(MeleeWeapon.drawDuringUse),
            new[]
            {
                typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), typeof(Farmer), typeof(Rectangle),
                typeof(int), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw weapon during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawDuringUsePrefix(
        Vector2 ___center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer f,
        Rectangle sourceRect,
        int type,
        bool isOnSpecial)
    {
        if (type == MeleeWeapon.stabbingSword && isOnSpecial)
        {
            DrawDuringStabbySwordLunge(
                ___center,
                frameOfFarmerAnimation,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
            return false; // don't run original logic
        }

        if (frameOfFarmerAnimation > 6 && ModEntry.State.ComboHitStep > ComboHitStep.FirstHit)
        {
            DrawDuringCombo(
                ___center,
                frameOfFarmerAnimation,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
            return false;  // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches

    private static void DrawDuringStabbySwordLunge(
        Vector2 center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer farmer,
        Rectangle sourceRectangle)
    {
        frameOfFarmerAnimation %= 2;
        switch (facingDirection)
        {
            case Game1.up:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 48f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32) / 10000f));
                        break;
                }

                break;
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 24f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
            case Game1.down:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 12f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 21f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                }

                break;
            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }
    }

    private static void DrawDuringCombo(
        Vector2 center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer farmer,
        Rectangle sourceRectangle)
    {
        var frame = farmer.FarmerSprite.CurrentFrame;
        switch (facingDirection)
        {
            case Game1.up:
                switch (frame)
                {
                    case 36:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 37:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 38:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 39:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 40:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 41:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                }

                break;

            case Game1.right:
                switch (frame)
                {
                    case 30:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 40f, playerPosition.Y - 64f + 8f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 31:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 56f, playerPosition.Y - 64f + 28f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 32:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 33:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 34:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 28f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 5f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 35:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            case Game1.down:
                switch (frame)
                {
                    case 24:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 25:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 26:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 40f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 27:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 28:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 29:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 12f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 9f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                }

                break;

            case Game1.left:
                switch (frame)
                {
                    case 30:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f, playerPosition.Y - 64f - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 31:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 48f, playerPosition.Y - 64f + 20f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 32:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 64f + 32f, playerPosition.Y + 16f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 33:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 34:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -5f / 8f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 35:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }
    }
}
