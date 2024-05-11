namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(
            nameof(SObject.draw),
            [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
    }

    #region harmony patches

    /// <summary>Patch to draw colored Slime Balls.</summary>
    [HarmonyPrefix]
    private static bool ObjectDrawPrefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        if (__instance.Name != "Slime Ball")
        {
            return true; // run original logic
        }

        var color = Color.Lime;
        var data = __instance.orderData.Value;
        if (!string.IsNullOrEmpty(data))
        {
            var split = data.Split('/');
            if (split.Length == 4 && uint.TryParse(split[0], out var packed))
            {
                color = new Color(packed);
            }
        }

        var (scaleX, scaleY) = __instance.getScale();
        var (posX, posY) = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, (y - 1) * Game1.tileSize));
        var destinationRect = new Rectangle(
            (int)(posX - (scaleX / 2f)),
            (int)(posY - (scaleY / 2f)),
            (int)(64f + scaleX),
            (int)(128f + (scaleY / 2f)));
        var drawLayer = Math.Max(0f, (((y + 1) * Game1.tileSize) - 24) / 10000f) + (x * 1e-5f);
        spriteBatch.Draw(
            Game1.bigCraftableSpriteSheet,
            destinationRect,
            SObject.getSourceRectForBigCraftable(__instance.showNextIndex.Value
                ? __instance.ParentSheetIndex + 1
                : __instance.ParentSheetIndex),
            color * alpha,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            drawLayer);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
