namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using AssetLoaders;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDrawWhenHeldPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDrawWhenHeldPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<SObject>(nameof(SObject.drawWhenHeld),
                new[] {typeof(SpriteBatch), typeof(Vector2), typeof(Farmer)})
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads when held.</summary>
    /// <remarks>Credit to <c>SilentOak</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool ObjectDrawWhenHeldPrefix(SObject __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
        if (__instance is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetMeadSourceRect(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: objectPosition,
            sourceRectangle: sourceRect,
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f)
        );

        if (f.ActiveObject == null || !f.ActiveObject.Name.Contains("=")) return false; // don't run original logic

        spriteBatch.Draw(
            texture: Textures.HoneyMeadTx,
            position: objectPosition + new Vector2(32f, 32f),
            sourceRectangle: sourceRect,
            color: Color.White,
            rotation: 0f,
            origin: new(32f, 32f),
            scale: 4f + Math.Abs(Game1.starCropShimmerPause) / 8f,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f)
        );

        if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97) return false; // don't run original logic
        
        Game1.starCropShimmerPause += 0.04f;
        if (Game1.starCropShimmerPause >= 0.8f) Game1.starCropShimmerPause = -0.8f;

        return false; // don't run original logic
    }
    
    #endregion harmony patches
}
