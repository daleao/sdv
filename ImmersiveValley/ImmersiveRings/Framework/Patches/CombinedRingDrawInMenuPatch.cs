namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingDrawInMenuPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingDrawInMenuPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool)
            });
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Draw gemstones on combined iridium band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingDrawInMenuPrefix(CombinedRing __instance, SpriteBatch spriteBatch, Vector2 location,
        float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color,
        bool drawShadow)
    {
        if (__instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return true; // run original logic

        try
        {
            var count = __instance.combinedRings.Count;
            if (count is < 1 or > 4)
                ThrowHelper.ThrowInvalidOperationException("Unexpected number of combined rings.");

            var oldScaleSize = scaleSize;
            scaleSize = 1f;
            location.Y -= (oldScaleSize - 1f) * 32f;

            // draw left half
            var src = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                __instance.indexInTileSheet.Value, 16, 16);
            src.X += 5;
            src.Y += 7;
            src.Width = 4;
            src.Height = 6;
            spriteBatch.Draw(Game1.objectSpriteSheet,
                location + new Vector2(51f, 51f) * scaleSize + new Vector2(-12f, 8f) * scaleSize, src,
                color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None,
                layerDepth);
            src.X++;
            src.Y += 4;
            src.Width = 3;
            src.Height = 1;
            spriteBatch.Draw(Game1.objectSpriteSheet,
                location + new Vector2(51f, 51f) * scaleSize + new Vector2(-8f, 4f) * scaleSize, src,
                color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None,
                layerDepth);

            // draw right half
            src = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.indexInTileSheet.Value,
                16, 16);
            src.X += 9;
            src.Y += 7;
            src.Width = 4;
            src.Height = 6;
            spriteBatch.Draw(Game1.objectSpriteSheet,
                location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 8f) * scaleSize, src,
                color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None,
                layerDepth);
            src.Y += 4;
            src.Width = 3;
            src.Height = 1;
            spriteBatch.Draw(Game1.objectSpriteSheet,
                location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 4f) * scaleSize, src,
                color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None,
                layerDepth);

            RingDrawInMenuPatch.RingDrawInMenuReverse(__instance, spriteBatch, location + new Vector2(-5f, -1f),
                scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);

            Vector2 offset;

            // draw top gem
            color = Utils.ColorByGemstone[__instance.combinedRings[0].ParentSheetIndex] * transparency;
            offset = ModEntry.IsBetterRingsLoaded ? new Vector2(19f, 3f) : new(23f, 11f);
            spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                new Rectangle(0, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None, layerDepth);

            if (count > 1)
            {
                // draw bottom gem (or left, in case of better rings)
                color = Utils.ColorByGemstone[__instance.combinedRings[1].ParentSheetIndex] * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(23f, 19f) : new(23f, 43f);
                spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                    new Rectangle(4, 0, 4, 4), color, 0, Vector2.Zero, scaleSize * 4f, SpriteEffects.None,
                    layerDepth);
            }

            if (count > 2)
            {
                // draw left gem (or right, in case of better rings)
                color = Utils.ColorByGemstone[__instance.combinedRings[2].ParentSheetIndex] * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(35f, 7f) : new(7f, 27f);
                spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                    new Rectangle(8, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None, layerDepth);
            }

            if (count > 3)
            {
                // draw right gem (or bottom, in case of better rings)
                color = Utils.ColorByGemstone[__instance.combinedRings[3].ParentSheetIndex] * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(39f, 23f) : new(39f, 27f);
                spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                    new Rectangle(12, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None,
                    layerDepth);
            }

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