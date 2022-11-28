namespace DaLion.Ligo.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotDrawPatcher"/> class.</summary>
    internal CrabPotDrawPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(
            nameof(CrabPot.draw),
            new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) });
    }

    #region harmony patches

    /// <summary>Patch to draw weapons in Luremaster crabpots.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDrawPrefix(
        CrabPot __instance, Vector2 ___shake, ref float ___yBob, SpriteBatch spriteBatch, int x, int y)
    {
        try
        {
            if (!__instance.readyForHarvest.Value || __instance.heldObject.Value is null ||
                !__instance.heldObject.Value.ParentSheetIndex.IsIn(14, 51))
            {
                return true; // run original logic
            }

            __instance.tileIndexToShow = 714;
            ___yBob = (float)((Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500d) + (x * 64)) *
                               8d) + 8d);
            if (___yBob <= 0.001f)
            {
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                    PathUtilities.NormalizeAssetName("TileSheets/animations"),
                    new Rectangle(0, 0, 64, 64),
                    150f,
                    8,
                    0,
                    __instance.directionOffset.Value + new Vector2((x * 64f) + 4f, (y * 64f) + 32f),
                    false,
                    Game1.random.NextDouble() < 0.5,
                    0.001f,
                    0.01f,
                    Color.White,
                    0.75f,
                    0.003f,
                    0f,
                    0f));
            }

            _ = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y];
            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2(x * 64f, (y * 64f) + ___yBob)) + ___shake,
                Game1.getSourceRectForStandardTileSheet(
                    Game1.objectSpriteSheet,
                    __instance.tileIndexToShow,
                    16,
                    16),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                ((y * 64) + __instance.directionOffset.Value.Y + (x % 4)) / 10000f);

            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset.Value + new Vector2((x * 64f) + 4f, (y * 64f) + 48f)) + ___shake,
                new Rectangle(
                    Game1.currentLocation.waterAnimationIndex * 64,
                    2112 + ((x + y) % 2 != 0 ? !Game1.currentLocation.waterTileFlip ? 128 : 0 : Game1.currentLocation.waterTileFlip ? 128 : 0),
                    56,
                    16 + (int)___yBob),
                Game1.currentLocation.waterColor.Value,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                ((y * 64) + __instance.directionOffset.Value.Y + (x % 4)) / 9999f);

            var yOffset = 4f *
                          (float)Math.Round(
                              Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250d), 2);
            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(
                    Game1.viewport,
                    __instance.directionOffset.Value + new Vector2((x * 64f) - 8f, (y * 64f) - 96f - 16f + yOffset)),
                new Rectangle(141, 465, 20, 24),
                Color.White * 0.75f,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                ((y + 1) * 64 / 10000f) + 1E-06f + (__instance.TileLocation.X / 10000f));

            spriteBatch.Draw(
                Tool.weaponsTexture,
                Game1.GlobalToLocal(
                    Game1.viewport,
                    __instance.directionOffset.Value + new Vector2((x * 64f) + 32f, (y * 64f) - 64f - 8f + yOffset)),
                Game1.getSourceRectForStandardTileSheet(
                    Tool.weaponsTexture,
                    __instance.heldObject.Value.ParentSheetIndex,
                    16,
                    16),
                Color.White * 0.75f,
                0f,
                new Vector2(8f, 8f),
                4f,
                SpriteEffects.None,
                ((y + 1) * 64 / 10000f) + 1E-05f + (__instance.TileLocation.X / 10000f));

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
