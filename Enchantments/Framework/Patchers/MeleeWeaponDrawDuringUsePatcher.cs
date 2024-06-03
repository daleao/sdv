namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawDuringUsePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawDuringUsePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MeleeWeaponDrawDuringUsePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(
            nameof(MeleeWeapon.drawDuringUse),
            [
                typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), typeof(Farmer), typeof(string),
                typeof(int), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Draw during stabby lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawDuringUsePrefix(
        Vector2 ___center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer f,
        string weaponItemId,
        int type,
        bool isOnSpecial)
    {
        if (!f.IsLocalPlayer || type != MeleeWeapon.defenseSword || !isOnSpecial)
        {
            return true; // run original logic
        }

        try
        {
            var sword = (MeleeWeapon)f.CurrentTool;
            if (!sword.hasEnchantmentOfType<StabbingEnchantment>())
            {
                return true; // run original logic
            }

            var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(weaponItemId);
            var texture = dataOrErrorItem.GetTexture() ?? Tool.weaponsTexture;
            var sourceRect = dataOrErrorItem.GetSourceRect();
            DrawDuringThrust(
                ___center,
                frameOfFarmerAnimation,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    private static void DrawDuringThrust(
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
                            Math.Max(0f, (farmer.StandingPixel.Y - 32) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y - 32) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 64) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 64) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 32) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 32) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 64) / 10000f));
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
                            Math.Max(0f, (farmer.StandingPixel.Y + 64) / 10000f));
                        break;
                }

                break;
        }
    }
}
