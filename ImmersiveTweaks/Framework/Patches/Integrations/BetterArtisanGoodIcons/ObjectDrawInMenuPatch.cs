namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.BetterArtisanGoodIcons;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDrawInMenuPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDrawInMenuPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<SObject>(nameof(SObject.drawInMenu),
                new[]
                {
                    typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                    typeof(StackDrawType), typeof(Color), typeof(bool)
                })
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads in menu.</summary>
    /// <remarks>Credit to <c>SilentOak</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool ObjectDrawInMenuPrefix(SObject __instance, SpriteBatch spriteBatch, Vector2 location,
        float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (__instance is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetSourceRectForMead(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        if (drawShadow)
        {
            var shadowTexture = Game1.shadowTexture;
            spriteBatch.Draw(
                texture: shadowTexture,
                position: location + new Vector2(32f, 48f),
                sourceRectangle: shadowTexture.Bounds,
                color: color * 0.5f,
                rotation: 0f,
                origin: new(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                scale: 3f,
                effects: SpriteEffects.None,
                layerDepth: layerDepth - 0.0001f
            );
        }

        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: location + new Vector2(
                (int) (32f * scaleSize),
                (int) (32f * scaleSize)
            ),
            sourceRectangle: sourceRect,
            color: color * transparency,
            rotation: 0f,
            origin: new Vector2(8f, 8f) * scaleSize,
            scale: 4f * scaleSize,
            effects: SpriteEffects.None,
            layerDepth: layerDepth
        );

        var shouldDrawStackNumber =
            (drawStackNumber == StackDrawType.Draw && __instance.maximumStackSize() > 1 && __instance.Stack > 1 ||
             drawStackNumber == StackDrawType.Draw_OneInclusive) && scaleSize > 0.3 &&
            __instance.Stack != int.MaxValue;

        if (shouldDrawStackNumber)
        {
            Utility.drawTinyDigits(
                toDraw: __instance.Stack,
                spriteBatch,
                position: location + new Vector2(
                    64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize) + 3f * scaleSize,
                    64f - 18f * scaleSize + 1f
                ),
                scale: 3f * scaleSize,
                layerDepth: 1f,
                color
            );
        }

        if (drawStackNumber != StackDrawType.Hide && __instance.Quality > 0)
        {

            var num = __instance.Quality < 4
                ? 0f
                : ((float) Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f;

            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                position: location + new Vector2(12f, 52f + num),
                sourceRectangle: __instance.Quality < 4
                    ? new(338 + (__instance.Quality - 1) * 8, 400, 8, 8)
                    : new(346, 392, 8, 8),
                color: color * transparency,
                rotation: 0f,
                origin: new(4f, 4f),
                scale: 3f * scaleSize * (1f + num),
                effects: SpriteEffects.None,
                layerDepth: layerDepth
            );
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}