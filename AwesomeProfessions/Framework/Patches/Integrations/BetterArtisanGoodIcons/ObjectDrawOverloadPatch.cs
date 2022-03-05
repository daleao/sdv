namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.BetterArtisanGoodIcons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using Utility;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDrawOverloadPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDrawOverloadPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<SObject>(nameof(SObject.draw),
                new[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(float)})
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads.</summary>
    /// <remarks>Credit to <c>SilentOak</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool ObjectDrawOverloadPrefix(SObject __instance, SpriteBatch spriteBatch, int xNonTile, int yNonTile,
        float layerDepth, float alpha = 1f)
    {
        if (__instance is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetSourceRectForMead(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        if (__instance.isTemporarilyInvisible || Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
            return false; // don't run original logic

        if (__instance.Fragility != 2)
        {
            var shadowTexture = Game1.shadowTexture;
            spriteBatch.Draw(
                texture: shadowTexture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(xNonTile + 32, yNonTile + 51 + 4)
                ),
                sourceRectangle: shadowTexture.Bounds,
                color: Color.White * alpha,
                rotation: 0f,
                origin: new(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: layerDepth - 1E-06f
            );
        }

        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    xNonTile + 32 + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0),
                    yNonTile + 32 + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)
                )
            ),
            sourceRectangle: sourceRect,
            color: Color.White * alpha,
            rotation: 0f,
            origin: new(8f, 8f),
            scale: __instance.Scale.Y > 1f ? __instance.getScale().Y : 4f,
            effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            layerDepth: layerDepth
        );

        return false; // don't run original logic
    }

    #endregion harmony patches
}