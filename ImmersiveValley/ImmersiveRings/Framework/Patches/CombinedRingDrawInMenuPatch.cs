namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System;
using System.Reflection;
using CommunityToolkit.Diagnostics;
using DaLion.Common;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingDrawInMenuPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingDrawInMenuPatch"/> class.</summary>
    internal CombinedRingDrawInMenuPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(
            nameof(CombinedRing.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Draw gemstones on combined Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingDrawInMenuPrefix(
        CombinedRing __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        if (__instance.ParentSheetIndex != Constants.IridiumBandIndex)
        {
            return true; // run original logic
        }

        try
        {
            var count = __instance.combinedRings.Count;
            if (count is < 1 or > 4)
            {
                ThrowHelper.ThrowInvalidOperationException("Unexpected number of combined rings.");
            }

            var oldScaleSize = scaleSize;
            scaleSize = 1f;
            location.Y -= (oldScaleSize - 1f) * 32f;

            RingDrawInMenuPatch.RingDrawInMenuReverse(
                __instance,
                spriteBatch,
                location,
                scaleSize,
                transparency,
                layerDepth,
                drawStackNumber,
                color,
                drawShadow);

            Vector2 offset;

            // draw top gem
            color = Gemstone.FromRing(__instance.combinedRings[0].ParentSheetIndex).Color * transparency;
            offset = ModEntry.IsBetterRingsLoaded ? new Vector2(19f, 3f) : new Vector2(24f, 12f);
            spriteBatch.Draw(
                Textures.GemstonesTx,
                location + (offset * scaleSize),
                new Rectangle(0, 0, 4, 4),
                color,
                0f,
                Vector2.Zero,
                scaleSize * 4f,
                SpriteEffects.None,
                layerDepth);

            if (count > 1)
            {
                // draw bottom gem (or left, in case of better rings)
                color = Gemstone.FromRing(__instance.combinedRings[1].ParentSheetIndex).Color * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(23f, 19f) : new Vector2(24f, 44f);
                spriteBatch.Draw(
                    Textures.GemstonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(4, 0, 4, 4),
                    color,
                    0,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
            }

            if (count > 2)
            {
                // draw left gem (or right, in case of better rings)
                color = Gemstone.FromRing(__instance.combinedRings[2].ParentSheetIndex).Color * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(35f, 7f) : new Vector2(8f, 28f);
                spriteBatch.Draw(
                    Textures.GemstonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(8, 0, 4, 4),
                    color,
                    0f,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
            }

            if (count > 3)
            {
                // draw right gem (or bottom, in case of better rings)
                color = Gemstone.FromRing(__instance.combinedRings[3].ParentSheetIndex).Color * transparency;
                offset = ModEntry.IsBetterRingsLoaded ? new Vector2(39f, 23f) : new Vector2(40f, 28f);
                spriteBatch.Draw(
                    Textures.GemstonesTx,
                    location + (offset * scaleSize),
                    new Rectangle(12, 0, 4, 4),
                    color,
                    0f,
                    Vector2.Zero,
                    scaleSize * 4f,
                    SpriteEffects.None,
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
