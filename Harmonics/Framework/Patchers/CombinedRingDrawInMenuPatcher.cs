namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingDrawInMenuPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CombinedRingDrawInMenuPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CombinedRing>(
            nameof(CombinedRing.drawInMenu),
            [
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            ]);
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Draw gemstones on combined Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingDrawInMenuPrefix(
        CombinedRing __instance,
        ref Vector2 __state,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        if (__instance.QualifiedItemId != $"(O){InfinityBandId}")
        {
            return true; // run original logic
        }

        try
        {
            var count = __instance.combinedRings.Count;
            if (count == 0)
            {
                // better rings needs to draw behind the gems
                RingDrawInMenuPatcher.RingDrawInMenuReverse(
                    __instance,
                    spriteBatch,
                    location,
                    scaleSize,
                    transparency,
                    layerDepth,
                    drawStackNumber,
                    color,
                    drawShadow);

                return false; // don't run original logic
            }

            var oldScaleSize = scaleSize;
            scaleSize = 1f;
            location.Y -= (oldScaleSize - 1f) * 32f;

            var usingBetterRings = RingTextureStyle == TextureStyle.BetterRings;
            var usingVanillaTweaks = RingTextureStyle == TextureStyle.VanillaTweaks;

            // better rings needs to draw the ring underneath the gems, but after the scale hover is converted to y-displacement
            if (usingBetterRings)
            {
                RingDrawInMenuPatcher.RingDrawInMenuReverse(
                    __instance,
                    spriteBatch,
                    location,
                    scaleSize,
                    transparency,
                    layerDepth,
                    drawStackNumber,
                    color,
                    drawShadow);
            }

            Vector2 offset;

            // draw top gem
            var gemColor = Gemstone.FromRing(__instance.combinedRings[0].QualifiedItemId).StoneColor * transparency;
            offset = usingVanillaTweaks
                ? new Vector2(24f, 6f)
                : usingBetterRings
                    ? new Vector2(24f, 4f) : new Vector2(24f, 12f);

            var sourceY = usingBetterRings ? 4 : 0;
            __state = location + (offset * scaleSize);
            spriteBatch.Draw(
                Textures.InfinityStonesTx,
                __state,
                new Rectangle(0, sourceY, 4, 4),
                gemColor,
                0f,
                Vector2.Zero,
                scaleSize * 4f,
                SpriteEffects.None,
                layerDepth);

            if (count > 1)
            {
                // draw bottom gem (or left, in case of better rings)
                gemColor = Gemstone.FromRing(__instance.combinedRings[1].QualifiedItemId).StoneColor * transparency;
                offset = usingBetterRings ? new Vector2(28f, 20f) : new Vector2(24f, 44f);
                spriteBatch.Draw(
                    Textures.InfinityStonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(4, sourceY, 4, 4),
                    gemColor,
                    0,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
            }

            if (count > 2)
            {
                // draw left gem (or right, in case of better rings)
                gemColor = Gemstone.FromRing(__instance.combinedRings[2].QualifiedItemId).StoneColor * transparency;
                offset = usingVanillaTweaks
                    ? new Vector2(3f, 25f)
                    : usingBetterRings
                        ? new Vector2(40f, 8f) : new Vector2(8f, 28f);
                spriteBatch.Draw(
                    Textures.InfinityStonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(8, sourceY, 4, 4),
                    gemColor,
                    0f,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
            }

            if (count > 3)
            {
                // draw right gem (or bottom, in case of better rings)
                gemColor = Gemstone.FromRing(__instance.combinedRings[3].QualifiedItemId).StoneColor * transparency;
                offset = usingVanillaTweaks
                    ? new Vector2(45f, 25f)
                    : usingBetterRings
                        ? new Vector2(44f, 24f) : new Vector2(40f, 28f);
                spriteBatch.Draw(
                    Textures.InfinityStonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(12, sourceY, 4, 4),
                    gemColor,
                    0f,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
            }

            // if better rings is not loaded, then the ring must be drawn over the gems
            if (!usingBetterRings)
            {
                RingDrawInMenuPatcher.RingDrawInMenuReverse(
                    __instance,
                    spriteBatch,
                    location,
                    scaleSize,
                    transparency,
                    layerDepth,
                    drawStackNumber,
                    color,
                    drawShadow);
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Draw gemstones on combined Infinity Band.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CombinedRingDrawInMenuPostfix(
        CombinedRing __instance,
        Vector2 __state,
        SpriteBatch spriteBatch,
        float scaleSize,
        float transparency,
        float layerDepth)
    {
        if (__instance.QualifiedItemId != $"(O){InfinityBandId}" ||
            __instance.combinedRings.Count == 0 || RingTextureStyle != TextureStyle.VanillaTweaks)
        {
            return;
        }

        if (Game1.activeClickableMenu is null)
        {
            __state.X += 2;
        }

        // redraw top gem
        var gemColor = Gemstone.FromRing(__instance.combinedRings[0].QualifiedItemId).StoneColor * transparency;
        spriteBatch.Draw(
            Textures.InfinityStonesTx,
            __state,
            new Rectangle(0, 0, 4, 4),
            gemColor,
            0f,
            Vector2.Zero,
            scaleSize * 4f,
            SpriteEffects.None,
            layerDepth);
    }

    #endregion harmony patches
}
