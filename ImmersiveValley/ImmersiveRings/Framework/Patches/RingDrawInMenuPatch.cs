#pragma warning disable SA1611
namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using DaLion.Common.Exceptions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawInMenuPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingDrawInMenuPatch"/> class.</summary>
    internal RingDrawInMenuPatch()
    {
        this.Target = this.RequireMethod<Ring>(
            nameof(Ring.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Ring.drawInMenu"/>.</summary>
    [HarmonyReversePatch]
    internal static void RingDrawInMenuReverse(
        object instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}
#pragma warning restore SA1611
