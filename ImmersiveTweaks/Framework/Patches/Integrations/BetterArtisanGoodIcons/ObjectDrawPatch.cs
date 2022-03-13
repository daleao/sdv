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
internal class ObjectDrawPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDrawPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<SObject>(nameof(SObject.draw),
                new[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)})
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads when held by machines.</summary>
    /// <remarks>Credit to <c>SilentOak</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool ObjectDrawPrefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
        if (!__instance.bigCraftable.Value || !__instance.readyForHarvest.Value ||
            __instance.heldObject.Value is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetSourceRectForMead(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        if (__instance.isTemporarilyInvisible) return false; // don't run original logic

        var (sx, sy) = __instance.getScale() * Game1.pixelZoom;

        var (px, py) = Game1.GlobalToLocal(
            viewport: Game1.viewport,
            globalPosition: new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize)
        );

        var destinationRect = new Rectangle(
            (int) (px - sx / 2f) + (__instance.shakeTimer > 0
                ? Game1.random.Next(-1, 2) : 0),
            (int) (py - sy / 2f) + (__instance.shakeTimer > 0
                ? Game1.random.Next(-1, 2) : 0),
            (int) (64f + sx),
            (int) (128f + sy / 2f)
        );

        spriteBatch.Draw(
            texture: Game1.bigCraftableSpriteSheet,
            destinationRectangle: destinationRect,
            sourceRectangle: SObject.getSourceRectForBigCraftable(__instance.showNextIndex.Value
                ? __instance.ParentSheetIndex + 1
                : __instance.ParentSheetIndex),
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f
        );

        var num = 4f * (float) Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
        spriteBatch.Draw(
            texture: Game1.mouseCursors,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    x * 64 - 8,
                    y * 64 - 96 - 16 + num
                )
            ),
            sourceRectangle: new Rectangle(141, 465, 20, 24),
            color: Color.White * 0.75f,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (float) ((y + 1) * 64 / 10000f + 1E-06f + __instance.TileLocation.X / 10000f + 9.99999997475243E-07 + __instance.TileLocation.X / 10000.0)
        );

        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    x * 64 + 32,
                    y * 64 - 64 - 8 + num
                )
            ),
            sourceRectangle: sourceRect,
            color: Color.White * 0.75f,
            rotation: 0f,
            origin: new(8f, 8f),
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (float) ((y + 1) * 64 / 10000f + __instance.TileLocation.X / 10000f + 9.99999974737875E-06 + __instance.TileLocation.X / 10000.0)
        );

        return false; // don't run original logic
    }

    #endregion harmony patches
}