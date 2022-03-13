namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.BetterArtisanGoodIcons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal class FurnitureDrawPatch : BasePatch
{
	/// <summary>Construct an instance.</summary>
    internal FurnitureDrawPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<Furniture>(nameof(Furniture.draw),
                new[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)})
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads on furniture.</summary>
    /// <remarks>Credit to <c>danvolchek (a.k.a. Cat)</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool FurnitureDrawPrefix(Furniture __instance, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y,
        float alpha = 1f)
    {
        if (__instance.heldObject.Value is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetSourceRectForMead(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        // draw the furniture
        if (x == -1)
        {
            spriteBatch.Draw(
                texture: Furniture.furnitureTexture,
                position: Game1.GlobalToLocal(Game1.viewport, ___drawPosition),
                sourceRectangle: __instance.sourceRect.Value,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }
        else
        {
            spriteBatch.Draw(
                texture: Furniture.furnitureTexture,
                position: Game1.GlobalToLocal(Game1.viewport,
                    globalPosition: new Vector2(
                        x: x * 64,
                        y: y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height))),
                sourceRectangle: __instance.sourceRect.Value,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }

        // draw shadow
        spriteBatch.Draw(
            texture: Game1.shadowTexture,
            position: Game1.GlobalToLocal(Game1.viewport, 
                globalPosition: new Vector2(
                    x: __instance.boundingBox.Value.Center.X - 32,
                    y: __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))) +
                new Vector2(32f, 53.3333321f),
            sourceRectangle: Game1.shadowTexture.Bounds,
            color: Color.White * alpha,
            rotation: 0f,
            origin: new(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
            scale: 4f,
            effects: SpriteEffects.None, __instance.boundingBox.Value.Bottom / 10000f
        );
        
        // draw the held item
        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: Game1.GlobalToLocal(Game1.viewport,
                globalPosition: new Vector2(
                    x: __instance.boundingBox.Value.Center.X - 32,
                    y: __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))),
            sourceRect,
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (__instance.boundingBox.Value.Bottom + 1) / 10000f
        );
        
        return false; // run original logic
    }

    #endregion harmony patches
}